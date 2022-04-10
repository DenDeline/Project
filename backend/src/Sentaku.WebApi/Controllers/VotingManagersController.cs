using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result.AspNetCore;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.SharedKernel.Constants;
using Sentaku.SharedKernel.Interfaces;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Models;
using Sentaku.SharedKernel.Models.VotingManagers;
using Sentaku.WebApi.Extensions;
using Swashbuckle.AspNetCore.Annotations;

namespace Sentaku.WebApi.Controllers;

[ApiController]
[Authorize]
[Route("/managers")]
public class VotingManagersController: ControllerBase
{
  private readonly IRepository<VotingManager> _votingManagerRepository;
  private readonly IRoleService _roleService;
  private readonly IMapper _mapper;
  private readonly AppDbContext _context;
  private readonly UserManager<AppUser> _userManager;

  public VotingManagersController(
    IRepository<VotingManager> votingManagerRepository,
    IRoleService roleService,
    IMapper mapper,
    AppDbContext context,
    UserManager<AppUser> userManager)
  {
    _votingManagerRepository = votingManagerRepository;
    _roleService = roleService;
    _mapper = mapper;
    _context = context;
    _userManager = userManager;
  }
  
  [HttpPost]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status201Created)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "User not found")]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<VotingManager>> CreateVotingManager(
    [FromBody] CreateVotingManagerRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.CreateVotingManagerByUsernameAsync(request.Username, cancellationToken);

    if (!result.IsSuccess)
      return this.ToActionResult(result);

    return CreatedAtAction(nameof(GetVotingManagerByUsername), new { managerId = result.Value.Id }, result.Value);
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<ListVotingManagersResponse>>> ListVotingManagers(
    CancellationToken cancellationToken)
  {
    var query =
      from votingManager in _context.VotingManagers
      join user in _context.Users on votingManager.IdentityId equals user.Id
      select new ListVotingManagersResponse
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
  
  [HttpGet("{managerId:guid}")]
  [SwaggerResponse(StatusCodes.Status200OK)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  public async Task<ActionResult<GetVotingManagerByUsernameResponse>> GetVotingManagerByUsername(
    [FromRoute] Guid managerId,
    CancellationToken cancellationToken)
  {
    var votingManager = await _votingManagerRepository.GetByIdAsync(managerId, cancellationToken);

    if (votingManager is null)
      return NotFound();
    
    var user = await _userManager.FindByIdAsync(votingManager.IdentityId);

    if (user is null)
      return NotFound();

    var response = new GetVotingManagerByUsernameResponse
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
      IsArchived = votingManager.IsArchived,
      ArchivedOn = votingManager.ArchivedOn
    };
    
    return Ok(response);
  }
  
  [HttpPut("{managerId:guid}/archive")]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successfully archived")]
  public async Task<ActionResult> UpdateArchiveVotingManagerById(
    [FromRoute] Guid managerId,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.ArchiveVotingManagerByIdAsync(managerId, cancellationToken);

    return this.ToActionResult(result);
  }
  
  [HttpPut("{managerId:guid}/restore")]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successfully restored")]
  public async Task<ActionResult> UpdateRestoreVotingManagerById(
    [FromRoute] Guid managerId,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.RestoreVotingManagerByIdAsync(managerId, cancellationToken);

    return this.ToActionResult(result);
  }
  
  [HttpDelete("{managerId:guid}")]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successfully deleted")]
  public async Task<ActionResult> DeleteVotingManagerById(
    [FromRoute] Guid managerId,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.DeleteVotingManagerByIdAsync(managerId, cancellationToken);

    return this.ToActionResult(result);
  }
}
