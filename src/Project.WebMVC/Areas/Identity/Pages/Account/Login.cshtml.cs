using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Project.WebMVC.Identity;

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

        public string ReturnUrl { get; set; }
            
        public IEnumerable<AuthenticationScheme> ExternalProviders { get; set; }
        
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

        public async Task OnGet(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            var signInResult = await _signInManager.PasswordSignInAsync(Input.Username, Input.Password, false, false);

            if (signInResult.Succeeded)
            {
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("Input.Username", "Fail to login");
            
            return Page();
        }
    }
}
