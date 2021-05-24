using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Project.WebMVC.Identity;
using Project.WebMVC.ViewModels;

namespace Project.WebMVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet("{username}")]
        public async Task<IActionResult> GetProfileByUsername(string username)
        {   
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return NotFound(username);
            }
            var vm = new ProfileViewModel
            {
                Username = user.UserName,
                ProfileImageUrl = user.ProfileImageUrl,
                Birthday = user.Birthday
            };
            
            return View("Index", vm);
        }
    }
}