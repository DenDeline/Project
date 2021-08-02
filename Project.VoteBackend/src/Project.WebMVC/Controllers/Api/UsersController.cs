using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result.AspNetCore;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Entities;
using Project.ApplicationCore.Interfaces;
using Project.WebMVC.Models.Api.Users;

namespace Project.WebMVC.Controllers.Api
{
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<AppUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        // =============================================================================================================//
        // USERS COLLECTIONS                                                                                            //
        //==============================================================================================================//

        [HttpGet("/api/users")]
        public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetAllUsers(CancellationToken ctsToken)
        {
          var users = await _userManager.Users.ToListAsync(ctsToken);
          return Ok(_mapper.Map<IEnumerable<GetUserResponse>>(users));
        }
        
        // =============================================================================================================//
        // USER INFO                                                                                                    //
        //==============================================================================================================//
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/api/user")]
        public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);

            var dto = _mapper.Map<GetCurrentUserResponse>(user);

            return Ok(dto);
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
        
        // =============================================================================================================//
        // USER ROLES                                                                                                   //
        //==============================================================================================================//
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/api/user/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetCurrentUserRoles()
        {
          var user = await _userManager.GetUserAsync(User);
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

        [Authorize(Policy = "update:user:role")]
        [HttpPost("/api/users/{username}/roles")]
        public async Task<ActionResult<IReadOnlyList<string>>> UpdateUserRolesByName(
          [FromRoute] string username, 
          [FromBody] UpdateUserRolesRequest request,
          [FromServices] IRoleService roleService,
          CancellationToken cts = new CancellationToken())
        {
          var user = await _userManager.GetUserAsync(User);
          
          var result = await roleService.AddRolesByUserNameAsync(user, username, request.Roles, cts);
          
          return this.ToActionResult(result);
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete("/api/users/{username}/roles")]
        public async Task<ActionResult<IReadOnlyList<string>>> DeleteUserRolesByName(
          [FromRoute] string username, 
          [FromBody] UpdateUserRolesRequest request,
          [FromServices] IRoleService roleService,
          CancellationToken cts = new CancellationToken())
        {

          var user = await _userManager.GetUserAsync(User);
          var result = await roleService.RemoveRolesByUserNameAsync(user, username, request.Roles, cts);
          return this.ToActionResult(result);
        }
    }   
}
