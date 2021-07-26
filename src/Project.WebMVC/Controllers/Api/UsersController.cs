using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Entities;

namespace Project.WebMVC.Controllers.Api
{
    [ApiController]
    public class UsersController: ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public UsersController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        
        [NonAction]
        [HttpGet("/api/users")]
        public async Task<IActionResult> GetUserListPagination([FromQuery] int since = 0, [FromQuery] int perPage = 30)
        {
            if (perPage > 100)
            {
                return BadRequest();
            }
            
            //TODO: Create read dto for user model
            var users = await _userManager.Users.SkipWhile(user => user.Id <= since).Take(perPage).ToListAsync();
            return Ok(users);
        }
    }   
}