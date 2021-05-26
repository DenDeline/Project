using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.Infrastructure.Data;

namespace Project.WebMVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public LoginModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }
        
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public string ReturnUrl { get; set; }
            
        public List<AuthenticationScheme> ExternalProviders { get; set; }
        
        public async Task<IActionResult> OnGetAsync([FromQuery] string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalProviders = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var signInResult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, false);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index");
            }
            
            ModelState.AddModelError("Input.Username", "Fail to login");

            return await OnGetAsync(ReturnUrl);
        }
    }
}
