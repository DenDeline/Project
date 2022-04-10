using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Models.User;

namespace Sentaku.WebApi.Controllers.User;

[Authorize]
[ApiController]
public class UserRolesController : ControllerBase
{
  private readonly UserManager<AppUser> _userManager;

  public UserRolesController(UserManager<AppUser> userManager)
  {
    _userManager = userManager;
  }
  
  [HttpGet("/user/roles")]
  public async Task<ActionResult<GetUserRolesResponse>> GetCurrentUserRoles()
  {
    var userId = _userManager.GetUserId(User);
    
    if (userId is null)
      return Forbid();

    var user = await _userManager.FindByIdAsync(userId);
    
    var roles = await _userManager.GetRolesAsync(user);

    return Ok(new GetUserRolesResponse { Roles = roles });
  }

  [HttpGet("/users/{username}/roles")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(StatusCodes.Status404NotFound)]
  [ProducesDefaultResponseType]
  public async Task<ActionResult<GetUserRolesResponse>> GetUserRolesByName([FromRoute] string username)
  {
    var user = await _userManager.FindByNameAsync(username);
    if (user is null)
    {
      return NotFound();
    }
    var roles = await _userManager.GetRolesAsync(user);
    return Ok(new GetUserRolesResponse { Roles = roles });
  }
}
