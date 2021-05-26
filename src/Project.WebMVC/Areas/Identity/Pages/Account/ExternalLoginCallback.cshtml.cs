using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Identity;

namespace Project.WebMVC.Areas.Identity.Pages.Account
{
    public class ExternalLoginCallback : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public ExternalLoginCallback(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        public async Task<IActionResult> OnGet(string returnUrl)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                return RedirectToPage("Register", new { ReturnUrl = returnUrl });
            }
            
            var result = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, 
                info.ProviderKey, 
                false);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }
            
            return RedirectToPage(nameof(ExternalRegister), new { ReturnUrl = returnUrl });
        }
    }
}