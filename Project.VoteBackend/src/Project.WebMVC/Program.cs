using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.ApplicationCore.Entities;
using Project.Infrastructure.Data;
using Project.WebMVC.AuthServer;

namespace Project.WebMVC
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await using (var services = host.Services.CreateAsyncScope())
            {
                var roleManager = services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

                if (!await roleManager.RoleExistsAsync(RoleConstants.Administrator))
                {
                    var adminRole = new IdentityRole<int>(RoleConstants.Administrator);
                    await roleManager.CreateAsync(adminRole);
                };
               
                if (!await roleManager.RoleExistsAsync(RoleConstants.LeadManager))
                {
                    var leadManagerRole = new IdentityRole<int>(RoleConstants.LeadManager);
                    await roleManager.CreateAsync(leadManagerRole);
                };
                
                if (!await roleManager.RoleExistsAsync(RoleConstants.RepresentativeAuthority))
                {
                    var representativeAuthorityRole = new IdentityRole<int>(RoleConstants.RepresentativeAuthority);
                    await roleManager.CreateAsync(representativeAuthorityRole);
                };
                
                if (!await roleManager.RoleExistsAsync(RoleConstants.Authority))
                {
                    var authority = new IdentityRole<int>(RoleConstants.Authority);
                    await roleManager.CreateAsync(authority);
                };
                
                var dbContext = services.ServiceProvider.GetRequiredService<AppDbContext>();

                if (await dbContext.Languages.CountAsync() == 0)
                {
                    var language = new Language {Name = "English", Code = "en", Enabled = true, IsDefault = true};
                    await dbContext.Languages.AddAsync(language);
                    await dbContext.SaveChangesAsync();
                }
                
                var userManager = services.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                
                if (await userManager.FindByNameAsync("admin") is null)
                {
                    var defaultLanguage = await dbContext.Languages.FirstOrDefaultAsync(_ => _.IsDefault && _.Enabled);
                    var admin = new AppUser("admin")
                    {
                      Name = "admin",
                      Surname = "admin",
                      LanguageId = defaultLanguage?.Id ?? throw new NullReferenceException()
                    };
                    await userManager.CreateAsync(admin, "admin");
                    await userManager.AddToRoleAsync(admin, RoleConstants.Administrator);
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
