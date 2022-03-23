using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
  if (builder.Environment.IsDevelopment())
    options.EnableSensitiveDataLogging();
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

if (builder.Environment.IsDevelopment())
  builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped<IIdentityTokenClaimService, IdentityTokenClaimService>();

builder.Services.AddDataProtection();

builder.Services.AddAuthentication();
  // .AddGoogle(config =>
  // {
  //   IConfigurationSection googleConfig =  builder.Configuration.GetSection("Authentication:Google");
  //
  //   config.ClientId = googleConfig["ClientId"];
  //   config.ClientSecret = googleConfig["ClientSecret"];
  // });

builder.Services.ConfigureApplicationCookie(_ => _.LoginPath = new PathString("/login"));

builder.Services.AddIdentity<AppUser, AppRole>(config =>
  {
    if (builder.Environment.IsDevelopment())
    {
      config.Password.RequireDigit = false;
      config.Password.RequiredLength = 4;
      config.Password.RequireLowercase = false;
      config.Password.RequireUppercase = false;
      config.Password.RequiredUniqueChars = 1;
      config.Password.RequireNonAlphanumeric = false;
    }
  })
  .AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
  options.AddPolicy("Swagger", policyBuilder =>
  {
    policyBuilder.AllowAnyHeader();
    policyBuilder.WithOrigins("https://localhost:5001");
    policyBuilder.AllowAnyMethod();
  });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseMigrationsEndPoint();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCors("Swagger");

app.MapControllers();

await app.RunAsync();
