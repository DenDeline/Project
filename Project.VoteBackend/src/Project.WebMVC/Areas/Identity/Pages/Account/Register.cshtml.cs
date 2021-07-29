using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Entities;
using Project.Infrastructure.Data;

namespace Project.WebMVC.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public RegisterModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, 
            AppDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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

        public async Task<IActionResult> OnPostAsync(CancellationToken token)
        {
            var defaultLanguage =  await _context.Languages.FirstOrDefaultAsync(e => e.IsDefault, token);
            
            var user = new AppUser(Input.Username)
            {
                Email = Input.Email,
                Birthday = Input.Birthday,
                LanguageId = defaultLanguage.Id
            };
            
            var createResult = await _userManager.CreateAsync(user, Input.Password);
            
            await _userManager.AddClaimAsync(user, new Claim("lang", defaultLanguage.Code));
            
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
