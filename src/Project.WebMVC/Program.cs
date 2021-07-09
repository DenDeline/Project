using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Project.ApplicationCore.Entities;

namespace Project.WebMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var services = host.Services.CreateScope())
            {
                var roleManager = services.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                
                var adminRole = new IdentityRole(RoleConstants.Administrator);
                roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();

                var leadManagerRole = new IdentityRole(RoleConstants.LeadManager);
                roleManager.CreateAsync(leadManagerRole).GetAwaiter().GetResult();
                
                var representativeAuthorityRole = new IdentityRole(RoleConstants.RepresentativeAuthority);
                roleManager.CreateAsync(representativeAuthorityRole).GetAwaiter().GetResult();
                
                var authority = new IdentityRole(RoleConstants.Authority);
                roleManager.CreateAsync(authority).GetAwaiter().GetResult();
                
                var userManager = services.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                var admin = new AppUser("admin")
                {
                    LanguageId = 3
                };
                userManager.CreateAsync(admin, "admin").GetAwaiter().GetResult();

                userManager.AddToRoleAsync(admin, RoleConstants.Administrator).GetAwaiter().GetResult();
            }
            
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
