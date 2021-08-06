﻿using Microsoft.Extensions.DependencyInjection;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Services;

namespace Project.Infrastructure
{
  public static class InfrastructureExtensions
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
      services.AddScoped<IIdentityTokenClaimService, IdentityTokenClaimService>();
      services.AddScoped<IRoleService, RoleService>();
      services.AddScoped<IPermissionsService, PermissionsService>();

      return services;
    }
  }
}
