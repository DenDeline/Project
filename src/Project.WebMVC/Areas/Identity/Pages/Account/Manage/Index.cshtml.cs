using System;
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
            
            public string LanguageId { get; set; }
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
            
            if (Input.PhoneNumber != null && user.PhoneNumber != Input.PhoneNumber)
            {
                var phoneNumberResult =  await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (phoneNumberResult.Succeeded)
                {
                    _logger.LogInformation("Change phone number for user with id: {0}", user.Id);
                }
            }

            if (user.LanguageId.ToString() != Input.LanguageId)
            {
                var language = await _context.Languages.FindAsync(Convert.ToInt32(Input.LanguageId));
                
                if (language != null)
                {
                    user.LanguageId = language.Id;
                
                    var changeLanguageResult = await _userManager.UpdateAsync(user);

                    if (changeLanguageResult.Succeeded)
                    {
                        _logger.LogInformation("Change language for user with id: {0}", user.Id);
                    }
                
                    var oldLanguageClaim = User.FindFirst("lang");
                    var newLanguageClaim = new Claim("lang", language.Code);
                    await _userManager.ReplaceClaimAsync(user, oldLanguageClaim, newLanguageClaim);
                
                    await _signInManager.RefreshSignInAsync(user);
                }
            }

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
                .Select(language => new SelectListItem
                {
                    Text = language.Name,
                    Value = language.Id.ToString(),
                    Selected = language.Id == defaultLanguageId
                })
                .ToListAsync(token);
            return languages.AsReadOnly();
        }
    }
}