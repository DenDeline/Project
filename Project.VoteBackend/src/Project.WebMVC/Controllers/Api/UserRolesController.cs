using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;
using Project.SharedKernel.Constants;
using Project.WebMVC.Authorization.PermissionsAuthorization;
using Project.WebMVC.Models.Api.Users;

namespace Project.WebMVC.Controllers.Api
{
  [ApiController]
  public class UserRolesController: ControllerBase
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRolesController(
      UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }
    
    [Authorize]
    [HttpGet("/api/user/roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetCurrentUserRoles()
    {
      var user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Name));
      return Ok(await _userManager.GetRolesAsync(user));
    }

    [HttpGet("/api/users/{username}/roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserRolesByName([FromRoute] string username)
    {
      var user = await _userManager.FindByNameAsync(username);
      if (user is null)
      {
        return NotFound();
      }
      var roles = (await _userManager.GetRolesAsync(user)).ToList();
      return Ok(roles);
    }

    [RequirePermissions(Permissions.ManageUserRoles)]
    [HttpPost("/api/users/{username}/roles")]
    public async Task<ActionResult<IReadOnlyList<string>>> UpdateUserRolesByName(
      [FromRoute] string username, 
      [FromBody] UpdateUserRolesRequest request,
      [FromServices] IRoleService roleService,
      CancellationToken cts = new CancellationToken())
    {
      if (User.Identity.Name is null)
      {
        return Forbid();
      }
      
      var result = await roleService.AddRolesByUserNameAsync(User.Identity.Name, username, request.Roles, cts);
      
      return this.ToActionResult(result);
    }

    [RequirePermissions(Permissions.ManageUserRoles)]
    [HttpDelete("/api/users/{username}/roles")]
    public async Task<ActionResult<IReadOnlyList<string>>> DeleteUserRolesByName(
      [FromRoute] string username, 
      [FromBody] DeleteUserRolesRequest request,
      [FromServices] IRoleService roleService,
      CancellationToken cts = new CancellationToken())
    {

      if (User.Identity.Name is null)
      {
        return Forbid();
      }
      
      var result = await roleService.RemoveRolesByUserNameAsync(User.Identity.Name, username, request.Roles, cts);
      return this.ToActionResult(result);
    }
  }
}
