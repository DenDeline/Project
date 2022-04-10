using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result.AspNetCore;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Constants;
using Sentaku.SharedKernel.Interfaces;
using Sentaku.SharedKernel.Models;
using Sentaku.SharedKernel.Models.Voter;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;
using Sentaku.WebApi.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace Sentaku.WebApi.Controllers;

[ApiController]
[Route("/voters")]
public class VotersController: ControllerBase
{
  private readonly IRepository<Voter> _votersRepository;
  private readonly IRoleService _roleService;
  private readonly IMapper _mapper;
  private readonly AppDbContext _context;
  private readonly UserManager<AppUser> _userManager;

  public VotersController(
    IRepository<Voter> votersRepository,
    IRoleService roleService,
    IMapper mapper,
    AppDbContext context,
    UserManager<AppUser> userManager)
  {
    _votersRepository = votersRepository;
    _roleService = roleService;
    _mapper = mapper;
    _context = context;
    _userManager = userManager;
  }
  
  [HttpPost]
  [RequirePermissions(Permissions.ManageUserRoles)]
  [SwaggerResponse(StatusCodes.Status201Created)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User not found")]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<Voter>> CreateVoter(
    [FromBody] CreateVoterRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.CreateVoterByUsernameAsync(request.Username, cancellationToken);

    if (!result.IsSuccess)
      return this.ToActionResult(result);

    return CreatedAtAction(nameof(GetVoterById), new { voterId = result.Value.Id }, result.Value);
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ListVoterResponse>>> ListVoters(
    CancellationToken cancellationToken)
  {
    var query =
      from votingManager in _context.Voters
      join user in _context.Users on votingManager.IdentityId equals user.Id
      select new ListVoterResponse
      {
        Id = votingManager.Id,
        Identity = new UserDto
        {
          Id = user.Id,
          Name = user.Name,
          Surname = user.Surname,
          Username = user.UserName,
          Verified = user.Verified,
          CreatedAt = user.CreatedAt
        },
        ArchivedOn = votingManager.ArchivedOn,
        IsArchived = votingManager.IsArchived
      };

    var response = await query.ToListAsync(cancellationToken);

    return Ok(response);
  }
  
  [HttpGet("{voterId:guid}")]
  [SwaggerResponse(StatusCodes.Status200OK)]
  [SwaggerResponse(StatusCodes.Status404NotFound)]
  public async Task<ActionResult<GetVoterByIdResponse>> GetVoterById(
    [FromRoute] Guid voterId,
    CancellationToken cancellationToken)
  {
    var voter = await _votersRepository.GetByIdAsync(voterId, cancellationToken);

    if (voter is null)
      return NotFound();
    
    var user = await _userManager.FindByIdAsync(voter.IdentityId);

    if (user is null)
      return NotFound();

    var response = new GetVoterByIdResponse
    {
      Id = voter.Id,
      Identity = new UserDto
      {
        Id = user.Id,
        Name = user.Name,
        Surname = user.Surname,
        Username = user.UserName,
        Verified = user.Verified,
        CreatedAt = user.CreatedAt
      },
      IsArchived = voter.IsArchived,
      ArchivedOn = voter.ArchivedOn
    };
    
    return Ok(response);
  }
  
  [HttpPut("{voterId:guid}/archive")]
  [RequirePermissions(Permissions.ManageUserRoles)]
  [SwaggerResponse(StatusCodes.Status404NotFound)]
  [SwaggerResponse(StatusCodes.Status204NoContent)]
  public async Task<ActionResult> UpdateArchiveVoterById(
    [FromRoute] Guid voterId,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.ArchiveVoterByIdAsync(voterId, cancellationToken);

    return this.ToActionResult(result);
  }
  
  [HttpPut("{voterId:guid}/restore")]
  [RequirePermissions(Permissions.ManageUserRoles)]
  [SwaggerResponse(StatusCodes.Status404NotFound)]
  [SwaggerResponse(StatusCodes.Status204NoContent)]
  public async Task<ActionResult> UpdateRestoreVoterById(
    [FromRoute] Guid voterId,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.RestoreVoterByIdAsync(voterId, cancellationToken);

    return this.ToActionResult(result);
  }
  
  [HttpDelete("{voterId:guid}")]
  [RequirePermissions(Permissions.ManageUserRoles)]
  [SwaggerResponse(StatusCodes.Status404NotFound)]
  [SwaggerResponse(StatusCodes.Status204NoContent)]
  public async Task<ActionResult> DeleteVoterById(
    [FromRoute] Guid voterId,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.DeleteVoterByIdAsync(voterId, cancellationToken);

    return this.ToActionResult(result);
  }
}
