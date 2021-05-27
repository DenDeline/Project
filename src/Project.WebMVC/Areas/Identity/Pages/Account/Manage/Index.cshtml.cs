using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.ApplicationCore.Entities;
using Project.Infrastructure.Data;

namespace Project.WebMVC.Areas.Identity.Pages.Account.Manage
{
    public class Index : PageModel
    {
        private readonly ILogger<Index> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public Index(
            ILogger<Index> logger,
            UserManager<AppUser> userManager, 
            SignInManager<AppUser> signInManager,
            AppDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        
        public IReadOnlyList<SelectListItem> Languages { get; set; }
        
        [BindProperty]
        public InputModel Input { get; set; }
        
        public class InputModel
        {
            public string Username { get; set; }
            
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }
            public string LanguageCode { get; set; }
        }
        
        public async Task OnGetAsync(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            
            Input = new InputModel
            {
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
            Languages = await GetEnabledLanguagesWithDefault(user.LanguageId, token);
        }

        public async Task OnPostAsync(CancellationToken token)
        {
            var user = await _userManager.GetUserAsync(User);
            var language = await _context.Languages.FirstOrDefaultAsync(e => e.Code == Input.LanguageCode, token);

            user.PhoneNumber = Input.PhoneNumber;

            if (language != null)
            {
                user.LanguageId = language.Id;
            }

            var oldLanguage = User.FindFirst("lang");
            await _userManager.UpdateAsync(user);

            if (language != null)
            {
                await _userManager.ReplaceClaimAsync(user, oldLanguage, new Claim("lang", language.Code));
            }

            await _signInManager.RefreshSignInAsync(user);
            
            Input = new InputModel
            {   
                Username = user.UserName,
                PhoneNumber = user.PhoneNumber
            };
            
            Languages = await GetEnabledLanguagesWithDefault(user.LanguageId, token);
        }

        [NonAction]
        private async Task<IReadOnlyList<SelectListItem>> GetEnabledLanguagesWithDefault(
            int defaultLanguageId, 
            CancellationToken token = default)
        {
            var languages = await _context.Languages
                .Where(language => language.Enabled)
                .ToListAsync(token);

            var userLanguage = languages.First(lang => lang.Id == defaultLanguageId);

            var selectListOfLanguages = languages.Select(lang => new SelectListItem(lang.Name, lang.Code)).ToList();

            var userLanguageSelectItem = selectListOfLanguages.First(item => item.Value == userLanguage.Code);

            userLanguageSelectItem.Selected = true;

            return selectListOfLanguages.AsReadOnly();
        }
    }
}