using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate.Specifications;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Constants;
using Sentaku.SharedKernel.Interfaces;
using Sentaku.SharedKernel.Models.VoteSession;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;

namespace Sentaku.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/sessions")]
public class VoteSessionsController: ControllerBase
{
  private readonly UserManager<AppUser> _userManager;
  private readonly IRepository<VotingManager> _votingManagerRepository;
  private readonly IRepository<VoteSession> _voteSessionRepository;
  private readonly IReadRepository<VoteSession> _voteSessionReadRepository;
  private readonly IRepository<Voter> _voterRepository;

  public VoteSessionsController(
    UserManager<AppUser> userManager,
    IRepository<VotingManager> votingManagerRepository,
    IRepository<VoteSession> voteSessionRepository,
    IReadRepository<VoteSession> voteSessionReadRepository,
    IRepository<Voter> voterRepository)
  {
    _userManager = userManager;
    _votingManagerRepository = votingManagerRepository;
    _voteSessionRepository = voteSessionRepository;
    _voteSessionReadRepository = voteSessionReadRepository;
    _voterRepository = voterRepository;
  }

  [HttpPost]
  [RequirePermissions(Permissions.ManageVotingSessions)]
  public async Task<ActionResult<VoteSession>> CreateVoteSession(
    [FromBody] CreateVoteSessionRequest request,
    CancellationToken cancellationToken = default)
  {
    var userId = _userManager.GetUserId(User);

    if (userId is null)
      return Unauthorized();

    var spec = new VotingManagerByIdentitySpec(userId);

    var votingManager = await _votingManagerRepository.GetBySpecAsync(spec, cancellationToken);
      
    if (votingManager is null)
      return Unauthorized(); 
    
    var sessionId = votingManager.CreateVoteSession(request.Agenda, request.StartDate);

    await _votingManagerRepository.SaveChangesAsync(cancellationToken);

    var session = await _voteSessionReadRepository.GetByIdAsync(sessionId, cancellationToken);

    return session is null ? Conflict() : CreatedAtAction(nameof(GetSessionById), new { sessionId }, session);
  }

  [HttpGet("{sessionId:guid}")]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  public async Task<ActionResult<VoteSession>> GetSessionById(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var session = await _voteSessionReadRepository.GetByIdAsync(sessionId, cancellationToken);

    if (session is null)
      return NotFound();
    
    return Ok(session);
  }
  
  [HttpGet]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  public async Task<ActionResult<IEnumerable<VoteSession>>> ListSessions(
    CancellationToken cancellationToken)
  {
    var session = await _voteSessionReadRepository.ListAsync(cancellationToken);
    return Ok(session);
  }
  
  [HttpPut("{sessionId:guid}/state/next")]
  [RequirePermissions(Permissions.ManageVotingSessions)]
  public async Task<ActionResult> UpdateMoveSessionInNextStateById(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var session = await _voteSessionRepository.GetByIdAsync(sessionId, cancellationToken);

    if (session is null)
      return NotFound();

    if (!session.MoveInNextState())
      return BadRequest();

    await _voteSessionRepository.SaveChangesAsync(cancellationToken);
    return NoContent();
  }
  
  [HttpGet("{sessionId:guid}/voters")]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  public async Task<ActionResult<IEnumerable<Guid>>> ListJoinVotersBySessionId(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var voteSessionSpec = new VoteSessionJoinVotersBySessionIdSpec(sessionId);
    
    var voters = await _voteSessionReadRepository.GetBySpecAsync(voteSessionSpec, cancellationToken);

    if (voters is null)
      return NotFound();
    
    return Ok(voters);
  }
  
  [HttpPut("{sessionId:guid}/join")]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  public async Task<ActionResult> UpdateJoinVoterBySessionId(
    [FromRoute] Guid sessionId,
    CancellationToken cancellationToken)
  {
    var userId = _userManager.GetUserId(User);

    if (userId is null)
      return Unauthorized();

    var voterSpec = new VoterByIdentitySpec(userId);

    var voter = await _voterRepository.GetBySpecAsync(voterSpec, cancellationToken);

    if (voter is null)
      return Forbid();

    var voteSessionSpec = new VoteSessionByIdIncludeQuestionsSpec(sessionId);
    
    var session = await _voteSessionRepository.GetBySpecAsync(voteSessionSpec, cancellationToken);

    if (session is null)
      return NotFound();

    if (!session.JoinVoter(voter))
      return BadRequest();

    await _voteSessionRepository.SaveChangesAsync(cancellationToken);
    
    return NoContent();
  }

  [HttpPut("{sessionId:guid}/vote")]
  [RequirePermissions(Permissions.ViewVotingSessions)]
  public async Task<ActionResult> UpdateVoteBySessionId(
    [FromRoute] Guid sessionId,
    [FromBody] UpdateVoteBySessionIdRequest request,
    CancellationToken cancellationToken)
  {
    var userId = _userManager.GetUserId(User);

    if (userId is null)
      return Unauthorized();

    var voterSpec = new VoterByIdentitySpec(userId);

    var voter = await _voterRepository.GetBySpecAsync(voterSpec, cancellationToken);

    if (voter is null)
      return Forbid();

    var voteSessionSpec = new VoteSessionByIdIncludeQuestionsAndVotersSpec(sessionId);
    
    var session = await _voteSessionRepository.GetBySpecAsync(voteSessionSpec, cancellationToken);

    if (session is null)
      return NotFound();

    if (!session.AddVotes(voter, request.Votes))
      return BadRequest();

    await _voteSessionRepository.SaveChangesAsync(cancellationToken);
    return NoContent();
  }
}
