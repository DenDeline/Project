using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Dtos.AppUser;
using Project.ApplicationCore.Entities;

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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/api/user")]
        public async Task<ActionResult> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(User);

            var dto = _mapper.Map<AppUserConfidentialReadDto>(user);

            return Ok(dto);
        }

        [HttpGet("/api/users/{username}/roles")]
        public async Task<ActionResult<IEnumerable<string>>> GetUserRoles(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return NotFound();
            }
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            return Ok(roles);
        }

        [HttpGet("/api/users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken ctsToken)
        {
            var users = await _userManager.Users.ToListAsync(ctsToken);
            return Ok(_mapper.Map<IEnumerable<AppUserReadDto>>(users));
        }

        // TODO: Implement pagination in the future
        [NonAction]
        [HttpGet("/api/users")]
        public async Task<IActionResult> GetUserListPagination([FromQuery] int since = 0, [FromQuery] int perPage = 30)
        {
            if (perPage > 100)
            {
                return BadRequest();
            }
            //TODO: Create read dto for user model
            var users = await _userManager.Users.Where(user => user.Id > since).Take(perPage).ToListAsync();
            return Ok(_mapper.Map<IEnumerable<AppUserReadDto>>(users));
        }
    }   
}