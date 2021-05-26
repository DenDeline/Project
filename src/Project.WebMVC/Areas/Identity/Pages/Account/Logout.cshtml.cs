using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;

namespace Project.WebMVC.Areas.Identity.Pages.Account
{
    public class Logout : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public Logout(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public async Task<IActionResult> OnPostAsync([FromRoute] string returnUrl = null)
        {
            await _signInManager.SignOutAsync();

            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }   
    }
}