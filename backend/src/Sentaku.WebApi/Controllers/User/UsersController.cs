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
using Sentaku.WebApi.Models.User;

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

  [HttpGet("/api/users")]
  public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetAllUsers(CancellationToken ctsToken)
  {
    var users = await _userManager.Users.ToListAsync(ctsToken);
    return Ok(_mapper.Map<IEnumerable<GetUserResponse>>(users));
  }
  
  [HttpGet("/api/user")]
  public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser()
  {
    if (User.Identity?.Name is null)
      return Forbid();

    var user = await _userManager.FindByNameAsync(User.Identity.Name);
    var response = _mapper.Map<GetCurrentUserResponse>(user);
    
    response.Roles = await _userManager.GetRolesAsync(user);

    var permissionResult =  await _permissionsService.GetPermissionsByUsernameAsync(User.Identity.Name);

    if (permissionResult.IsSuccess)
      response.Permissions = permissionResult.Value;

    return Ok(response);
  }

  [HttpGet("/api/users/{username}")]
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
