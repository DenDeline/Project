using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Aggregates;
using Project.SharedKernel.Constants;

namespace Project.Infrastructure.Data
{
  public class MockDatabase
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppDbContext _context;

    public MockDatabase(
      UserManager<ApplicationUser> userManager, 
      RoleManager<ApplicationRole> roleManager,
      AppDbContext context)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _context = context;
    }

    public async Task MockRolesAsync()
    {
      if (!await _roleManager.RoleExistsAsync(nameof(Roles.Administrator)))
      {
        var adminRole = new ApplicationRole(nameof(Roles.Administrator))
        {
          Position = 4,
          Permissions = Permissions.Administrator
        };
        await _roleManager.CreateAsync(adminRole);
      };
               
      if (!await _roleManager.RoleExistsAsync(nameof(Roles.LeadManager)))
      {
        var leadManagerRole = new ApplicationRole(nameof(Roles.LeadManager))
        {
          Position = 3,
          Permissions = Permissions.ManageUserRoles | Permissions.VerifyUsers | Permissions.ManageVotingSessions | Permissions.ViewVotingSessions
        };
        await _roleManager.CreateAsync(leadManagerRole);
      };
                
      if (!await _roleManager.RoleExistsAsync(nameof(Roles.RepresentativeAuthority)))
      {
        var representativeAuthorityRole = new ApplicationRole(nameof(Roles.RepresentativeAuthority))
        {
          Position = 2,
          Permissions = Permissions.ViewVotingSessions
        };
        await _roleManager.CreateAsync(representativeAuthorityRole);
      };
                
      if (!await _roleManager.RoleExistsAsync(nameof(Roles.Authority)))
      {
        var authority = new ApplicationRole(nameof(Roles.Authority))
        {
          Position = 1,
          Permissions = Permissions.ViewVotingSessions
        };
        await _roleManager.CreateAsync(authority);
      }
    }

    public async Task MockLanguagesAsync()
    {
      if (await _context.Languages.CountAsync() == 0)
      {
        var language = new Language {Name = "English", Code = "en", Enabled = true, IsDefault = true};
        await _context.Languages.AddAsync(language);
        await _context.SaveChangesAsync();
      }
    }

    public async Task MockUsersAsync()
    {
      var defaultLanguage = await _context.Languages.FirstOrDefaultAsync(_ => _.IsDefault && _.Enabled);
      
      if (await _userManager.FindByNameAsync("admin") is null)
      {
          var admin = new ApplicationUser("admin")
          {
            LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
          };
          admin.UpdateProfileInfo("admin","admin");
          admin.UpdateVerification(true);
          
          await _userManager.CreateAsync(admin, "admin");
          await _userManager.AddToRoleAsync(admin, nameof(Roles.Administrator));
      }
      
      if (await _userManager.FindByNameAsync("test_LM") is null)
      {
        var leadManager = new ApplicationUser("test_LM")
        {
          LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
        };
        leadManager.UpdateProfileInfo("Steve", "Smith");
        leadManager.UpdateVerification(true);
        await _userManager.CreateAsync(leadManager, "test_LM");
        await _userManager.AddToRoleAsync(leadManager, nameof(Roles.LeadManager));
      }
      
      if (await _userManager.FindByNameAsync("test_RA") is null)
      {
        var representativeAuthority = new ApplicationUser("test_RA")
        {
          LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
        };
        representativeAuthority.UpdateProfileInfo("Joe", "Conor");
        representativeAuthority.UpdateVerification(true);
        await _userManager.CreateAsync(representativeAuthority, "test_RA");
        await _userManager.AddToRoleAsync(representativeAuthority, nameof(Roles.RepresentativeAuthority));
      }
      
      if (await _userManager.FindByNameAsync("test_A") is null)
      {
        var representativeAuthority = new ApplicationUser("test_A")
        {
          LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
        };
        representativeAuthority.UpdateProfileInfo("Kristina", "Brown");
        representativeAuthority.UpdateVerification(true);
        await _userManager.CreateAsync(representativeAuthority, "test_A");
        await _userManager.AddToRoleAsync(representativeAuthority, nameof(Roles.Authority));
      }
      
      if (await _userManager.FindByNameAsync("defUser") is null)
      {
        var representativeAuthority = new ApplicationUser("defUser")
        {
          LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
        };
        representativeAuthority.UpdateProfileInfo( "Kenny", "Railway");
        
        await _userManager.CreateAsync(representativeAuthority, "defUser");
      }
    }
  }
}
