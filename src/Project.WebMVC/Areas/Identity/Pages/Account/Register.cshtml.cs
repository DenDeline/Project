using System;
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
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }
            
            [Required]
            [DataType(DataType.Date)]
            public DateTime Birthday { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
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
            var user = new AppUser(Input.Username)
            {
                Email = Input.Email,
                Birthday = Input.Birthday
            };

            var createResult = await _userManager.CreateAsync(user, Input.Password);
            
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return await OnGetAsync(ReturnUrl);
            }

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index");
        }
    }
}
