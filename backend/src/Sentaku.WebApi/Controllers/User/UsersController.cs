using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Constants;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;
using Sentaku.WebApi.Models.User;
using Microsoft.AspNetCore.Http;

namespace Sentaku.WebApi.Controllers.User;

[Authorize]
[ApiController]
public class UsersController : ControllerBase
{
  private readonly UserManager<AppUser> _userManager;
  private readonly IPermissionsService _permissionsService;
  private readonly IMapper _mapper;

  public UsersController(
    UserManager<AppUser> userManager,
    IPermissionsService permissionsService,
    IMapper mapper)
  {
    _userManager = userManager;
    _permissionsService = permissionsService;
    _mapper = mapper;
  }
  
  [RequirePermissions(Permissions.ViewUsers)]
  [HttpGet("/users")]
  public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetUsers(CancellationToken cancellationToken)
  {
    var response = await _mapper
      .ProjectTo<GetUserResponse>(_userManager.Users)
      .ToListAsync(cancellationToken);
    
    return Ok(response);
  }
  
  [HttpGet("/user")]
  public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser()
  {
    if (User.Identity?.Name is null)
      return Forbid();

    var user = await _userManager.FindByNameAsync(User.Identity.Name);
    var response = _mapper.Map<GetCurrentUserResponse>(user);
    
    response.Roles = await _userManager.GetRolesAsync(user);

    var permissionResult =  await _permissionsService.GetPermissionsAsync(User);

    if (!permissionResult.IsSuccess)
      return Forbid();
    
    response.Permissions = permissionResult.Value;

    return Ok(response);
  }

  [HttpGet("/users/{username}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<GetUserResponse>> GetUserByName([FromRoute] string username)
  {
    var user = await _userManager.FindByNameAsync(username);

    if (user is null)
    {
      return NotFound();
    }

    var dto = _mapper.Map<GetUserResponse>(user);

    return Ok(dto);
  }
}
