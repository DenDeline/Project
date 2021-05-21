using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NodaTime;
using Project.WebMVC.Identity;

namespace Project.WebMVC.Areas.Identity.Pages.Account
{
    public class ExternalRegister : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public ExternalRegister(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        public string ReturnUrl { get; set; }
        
        [BindProperty]
        public ExternalRegisterModel Input { get; set; }
        
        public class ExternalRegisterModel
        {
            [Required]
            public string Username { get; set; }
            
            [Required]
            [DataType(DataType.Date)]
            public LocalDate Birthday { get; set; }
            
            [Required]
            public string ReturnUrl { get; set; }

        }

        public void OnGet([FromQuery] string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToPage("Register");
            }
            
            var user = new AppUser(Input.Username)
            {
                Email = info.Principal.FindFirst(ClaimTypes.Email).Value,
                Birthday = Input.Birthday
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return Page();
            }

            result = await _userManager.AddLoginAsync(user, info);
            
            if (!result.Succeeded)
            {
                return Page();
            }

            await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);
            
            return Redirect(Input.ReturnUrl);
        }
    }
}