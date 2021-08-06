using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.WebMVC.Models.Api.Users;

namespace Project.WebMVC.Controllers.Api
{
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }
        
        [HttpGet("/api/users")]
        public async Task<ActionResult<IEnumerable<GetUserResponse>>> GetAllUsers(CancellationToken ctsToken)
        {
          var users = await _userManager.Users.ToListAsync(ctsToken);
          return Ok(_mapper.Map<IEnumerable<GetUserResponse>>(users));
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/api/user")]
        public async Task<ActionResult<GetCurrentUserResponse>> GetCurrentUser()
        {
            var user = await _userManager.FindByNameAsync(User.FindFirstValue(ClaimTypes.Name));

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
    }   
}
