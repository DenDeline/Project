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
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;
using Project.WebMVC.MappingProfiles;

namespace Project.WebMVC
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public string NetJSClientCorsPolicy = "NetJSClientCorsPolicy";
        
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddApplicationCore();

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddDbContext<AppDbContext>(options =>
            {
                if (Environment.IsDevelopment())
                {
                    options.EnableSensitiveDataLogging();
                }
                
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IDbContext, AppDbContext>();

            services.AddIdentity<AppUser, IdentityRole<int>>(config =>
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
                    var secretKey = new SigningIssuerCertificate().GetPublicKey();
                    
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = "https://localhost:44307",
                        ValidAudience = "https://localhost:44307",
                        IssuerSigningKey = secretKey,
                    };
                });
                
            services.AddCors(options =>
            {
                options.AddPolicy(NetJSClientCorsPolicy, builder =>
                {
                    builder.AllowAnyHeader();
                    builder.WithOrigins("http://localhost:3000");
                    builder.AllowAnyMethod();
                });
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

            app.UseCors(NetJSClientCorsPolicy);
            
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
