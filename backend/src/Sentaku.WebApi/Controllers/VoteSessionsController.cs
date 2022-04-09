using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Constants;
using Sentaku.SharedKernel.Interfaces;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;
using Sentaku.WebApi.Models.VoteSession;

namespace Sentaku.WebApi.Controllers;

[Authorize]
[ApiController]
[Route("/sessions")]
public class VoteSessionsController: ControllerBase
{
  private readonly UserManager<AppUser> _userManager;
  private readonly IRepository<VotingManager> _votingManagerRepository;
  private readonly IReadRepository<VoteSession> _voteSessionReadRepository;

  public VoteSessionsController(
    UserManager<AppUser> userManager,
    IRepository<VotingManager> votingManagerRepository,
    IReadRepository<VoteSession> voteSessionReadRepository)
  {
    _userManager = userManager;
    _votingManagerRepository = votingManagerRepository;
    _voteSessionReadRepository = voteSessionReadRepository;
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
}
