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
using Sentaku.WebApi.Models.VotingManagers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.WebApi.Models;
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
  [SwaggerOperation( 
    "Create voting manager", 
    "Create voting managers by identity username", 
    Tags = new [] { "VotingManagers" })]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<VotingManager>> CreateVotingManager(
    [FromBody] CreateVotingManagerRequest request,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.CreateVotingManagerByUsernameAsync(request.Username, cancellationToken);

    if (!result.IsSuccess)
      return this.ToActionResult(result);

    return CreatedAtAction(nameof(GetVotingManagerByUsername), new { username = request.Username }, result.Value);
  }

  [HttpGet]
  [SwaggerOperation( 
    "List voting manager", 
    "List all voting managers in system", 
    Tags = new [] { "VotingManagers", "ObserveDomain" })]
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
  
  [HttpGet("{username}")]
  [SwaggerResponse(StatusCodes.Status200OK)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerOperation( 
    "Find voting manager by username", 
    "Find voting manager by username", 
    Tags = new [] { "VotingManagers" })]
  public async Task<ActionResult<GetVotingManagerByUsernameResponse>> GetVotingManagerByUsername(
    [FromRoute] string username,
    CancellationToken cancellationToken)
  {
    var user = await _userManager.FindByNameAsync(username);

    if (user is null)
      return NotFound();
    
    var spec = new VotingManagerByIdentitySpec(user.Id);
    
    var votingManager = await _votingManagerRepository.GetBySpecAsync(spec, cancellationToken);

    if (votingManager is null)
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
  
  [HttpPut("{username}/archive")]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successfully archived")]
  [SwaggerOperation( 
    "Archive voting manager", 
    "Archive voting manager and remove LeadManager role", 
    Tags = new [] { "VotingManagers", "Archiving" })]
  public async Task<ActionResult> UpdateArchiveVotingManagerById(
    [FromRoute] string username,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.ArchiveVotingManagerByUsernameAsync(username, cancellationToken);

    return this.ToActionResult(result);
  }
  
  [HttpPut("{username}/restore")]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successfully restored")]
  [SwaggerOperation( 
    "Restore voting manager", 
    "Restore voting manager and add LeadManager role", 
    Tags = new [] { "VotingManagers", "Restoring" })]
  public async Task<ActionResult> UpdateRestoreVotingManagerById(
    [FromRoute] string username,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.RestoreVotingManagerByUsernameAsync(username, cancellationToken);

    return this.ToActionResult(result);
  }
  
  [HttpDelete("{username}")]
  [RequirePermissions(Permissions.Administrator)]
  [SwaggerResponse(StatusCodes.Status404NotFound, Description = "Vote manager not found")]
  [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successfully deleted")]
  [SwaggerOperation( 
    "Delete voting manager form system", 
    "Delete voting manager form system and associated information", 
    Tags = new [] { "VotingManagers", "PermanentDelete" })]
  public async Task<ActionResult> DeleteVotingManagerById(
    [FromRoute] string username,
    CancellationToken cancellationToken)
  {
    var result = await _roleService.DeleteVotingManagerByUsernameAsync(username, cancellationToken);

    return this.ToActionResult(result);
  }
}
