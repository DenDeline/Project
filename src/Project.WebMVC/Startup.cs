using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Project.ApplicationCore;
using Project.ApplicationCore.Entities;
using Project.Infrastructure.Data;

namespace Project.WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApplicationCore();

            services.AddDbContext<AppDbContext>(options => 
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<AppUser, IdentityRole>(config =>
                {
                    config.Password.RequireDigit = false;
                    config.Password.RequiredLength = 4;
                    config.Password.RequireLowercase = false;
                    config.Password.RequireUppercase = false;
                    config.Password.RequiredUniqueChars = 1;
                    config.Password.RequireNonAlphanumeric = false;
                })
                .AddDefaultUI()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddGoogle(config =>
                {
                    IConfigurationSection googleConfig =
                        Configuration.GetSection("Authentication:Google");

                    config.ClientId = googleConfig["ClientId"];
                    config.ClientSecret = googleConfig["ClientSecret"];
                })
                
                .AddJwtBearer(options =>
                {
                    IConfiguration projectConfig = Configuration.GetSection("Authentication:Project");
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(projectConfig["ClientSecret"]));
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "https://localhost:44307",
                        ValidAudience = "https://localhost:44307",
                        IssuerSigningKey = secretKey,
                    };
                });
                

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();  
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapRazorPages();
            });
        }
    }
}
