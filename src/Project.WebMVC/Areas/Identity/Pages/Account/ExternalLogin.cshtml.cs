using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.WebMVC.Identity;

namespace Project.WebMVC.Areas.Identity.Pages.Account
{
    public class ExternalLogin : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public ExternalLogin(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        [BindProperty]
        public ExternalLoginModel Input { get; set; }
        
        public class ExternalLoginModel
        {
            [Required]
            public string ReturnUrl { get; set; }
            
            [Required]
            public string Provider { get; set; }
        }
        
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var redirectUri = Url.Page(nameof(ExternalLoginCallback), new { Input.ReturnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(Input.Provider, redirectUri);

            return Challenge(properties, Input.Provider);
        }
    }
}