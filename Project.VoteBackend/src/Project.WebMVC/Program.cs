using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.ApplicationCore.Aggregates;
using Project.Infrastructure.Data;
using Project.SharedKernel.Constants;

namespace Project.WebMVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await using (var services = host.Services.CreateAsyncScope())
            {
                var roleManager = services.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                if (!await roleManager.RoleExistsAsync(nameof(Roles.Administrator)))
                {
                    var adminRole = new ApplicationRole(nameof(Roles.Administrator))
                    {
                      Position = 4,
                      Permissions = Permissions.Administrator
                    };
                    await roleManager.CreateAsync(adminRole);
                };
               
                if (!await roleManager.RoleExistsAsync(nameof(Roles.LeadManager)))
                {
                    var leadManagerRole = new ApplicationRole(nameof(Roles.LeadManager))
                    {
                      Position = 3,
                      Permissions = Permissions.ManageRoles
                    };
                    await roleManager.CreateAsync(leadManagerRole);
                };
                
                if (!await roleManager.RoleExistsAsync(nameof(Roles.RepresentativeAuthority)))
                {
                    var representativeAuthorityRole = new ApplicationRole(nameof(Roles.RepresentativeAuthority))
                    {
                      Position = 2,
                    };
                    await roleManager.CreateAsync(representativeAuthorityRole);
                };
                
                if (!await roleManager.RoleExistsAsync(nameof(Roles.Authority)))
                {
                    var authority = new ApplicationRole(nameof(Roles.Authority))
                    {
                      Position = 1
                    };
                    await roleManager.CreateAsync(authority);
                };
                
                var dbContext = services.ServiceProvider.GetRequiredService<AppDbContext>();

                if (await dbContext.Languages.CountAsync() == 0)
                {
                    var language = new Language {Name = "English", Code = "en", Enabled = true, IsDefault = true};
                    await dbContext.Languages.AddAsync(language);
                    await dbContext.SaveChangesAsync();
                }
                
                var userManager = services.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                
                if (await userManager.FindByNameAsync("admin") is null)
                {
                    var defaultLanguage = await dbContext.Languages.FirstOrDefaultAsync(_ => _.IsDefault && _.Enabled);
                    var admin = new ApplicationUser("admin")
                    {
                      Name = "admin",
                      Surname = "admin",
                      LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
                    };
                    await userManager.CreateAsync(admin, "admin");
                    await userManager.AddToRoleAsync(admin, nameof(Roles.Administrator));
                }
            }
            
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
