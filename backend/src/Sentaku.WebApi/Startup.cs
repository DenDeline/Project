﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Sentaku.ApplicationCore;
using Sentaku.Infrastructure;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel;
using Sentaku.WebApi.Authorization.PermissionsAuthorization;

namespace Sentaku.WebApi
{
  public class Startup
  {
    
    public const string NetJsClientCorsPolicy = "NetJSClientCorsPolicy";
    public const string ServiceName = "WebApiService";
    
    private readonly ResourceBuilder _resourceBuilder;

    public IConfiguration Configuration { get; }
    public IWebHostEnvironment Environment { get; }
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
      Configuration = configuration;
      Environment = environment;
      var assemblyVersion  = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
      
      _resourceBuilder = ResourceBuilder
        .CreateDefault()
        .AddService(ServiceName, serviceVersion: assemblyVersion, serviceInstanceId: System.Environment.MachineName);
    }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddApplicationCore();
      services.AddInfrastructure();

      services.AddAutoMapper(typeof(Startup).Assembly);

      services.AddOpenTelemetryTracing(builder =>
      {
        builder
          .SetResourceBuilder(_resourceBuilder)
          .AddConsoleExporter()
          .AddHttpClientInstrumentation()
          .AddAspNetCoreInstrumentation()
          .AddEntityFrameworkCoreInstrumentation();
      });

      services.AddLogging(builder =>
      {
        builder.ClearProviders(); 
        builder.AddOpenTelemetry(_ =>
        {
          _.SetResourceBuilder(_resourceBuilder);
          _.AddConsoleExporter();
        });
      });

      services.AddDbContext<AppDbContext>(options =>
      {
        if (Environment.IsDevelopment())
        {
          options
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
        }

        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
      });

      if (Environment.IsDevelopment())
        services.AddDatabaseDeveloperPageExceptionFilter();
      
      services.AddIdentityCore<AppUser>(config =>
        {
          if (Environment.IsDevelopment())
          {
            config.Password.RequireDigit = false;
            config.Password.RequiredLength = 4;
            config.Password.RequireLowercase = false;
            config.Password.RequireUppercase = false;
            config.Password.RequiredUniqueChars = 1;
            config.Password.RequireNonAlphanumeric = false;
          }
        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

      services.AddHttpContextAccessor();
      services.AddSingleton<IAuthorizationPolicyProvider, PermissionsPolicyProvider>();
      services.AddScoped<IAuthorizationHandler, PermissionsAuthorizationHandler>();

      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidIssuer = "https://localhost:7045",
            ValidAudience = "https://localhost:5001",
            IssuerSigningKey = new SigningIssuerCertificate().GetPublicKey()
          };

          options.Events = new JwtBearerEvents
          {
            OnTokenValidated = async context =>
            {
              if (context.Principal is null)
              {
                context.Fail("Principal is not authenticated");
                return;
              }

              var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

              var userId = context.Principal.FindFirstValue(userManager.Options.ClaimsIdentity.UserIdClaimType);
              var securityStamp = context.Principal.FindFirstValue(userManager.Options.ClaimsIdentity.SecurityStampClaimType);

              if (userId is null || securityStamp is null)
              {
                context.Fail("Invalid access token");
                return;
              }

              var userResponse = await userManager.Users
                .Where(_ => _.Id == userId)
                .Select(_ => new { _.SecurityStamp })
                .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

              if (userResponse is null)
              {
                context.Fail("User doesn't exist");
                return;
              }

              if (securityStamp != userResponse.SecurityStamp)
              {
                context.Fail("Principal is not authenticated");
                return;
              }
              
              context.Success();
            }
          };
        }); 

      services.AddCors(options =>
      {
        options.AddPolicy(NetJsClientCorsPolicy, builder =>
        {
          builder.AllowAnyHeader();
          builder.WithOrigins("http://localhost:3000");
          builder.AllowAnyMethod();
        });
      });

      services.AddControllers();

      services.AddEndpointsApiExplorer();

      services.AddSwaggerGen(c =>
      {
        c.EnableAnnotations();
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vote WebAPI", Version = "v1" });
        c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
          Type = SecuritySchemeType.OAuth2,
          Flows = new OpenApiOAuthFlows
          {
            AuthorizationCode = new OpenApiOAuthFlow
            {
              AuthorizationUrl = new Uri("https://localhost:7045/login/oauth2/authorize"),
              TokenUrl = new Uri("https://localhost:7045/login/oauth2/token")
            }
          }
        });
        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
          {
            new OpenApiSecurityScheme
            {
              Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            },
            new List<string>()
          }
        });
      });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
          c.OAuthClientId("project_swagger_3e1db73b647f43c297594797d62aec76");
          c.OAuthUsePkce();
          c.SwaggerEndpoint("v1/swagger.json", "My API V1");
        });
        
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }
      app.UseHttpsRedirection();

      app.UseCors(NetJsClientCorsPolicy);

      app.UseRouting();

      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapSwagger();
        endpoints.MapControllers();
      });
    }
  }
}
