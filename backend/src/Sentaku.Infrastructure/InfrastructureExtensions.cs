using System.Collections.Generic;
using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Sentaku.ApplicationCore;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.Infrastructure.Services;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.Infrastructure
{
  public static class InfrastructureExtensions
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
      services.AddScoped<IIdentityTokenClaimService, IdentityTokenClaimService>();
      services.AddScoped<IRoleService, RoleService>();
      services.AddScoped<IPermissionsService, PermissionsService>();
      services.AddScoped<IUserVerificationService, UserVerificationService>();

      var assemblies = new List<Assembly>();
      
      var coreAssembly = Assembly.GetAssembly(typeof(ApplicationCoreExtensions));
      var infrastructureAssembly = Assembly.GetAssembly(typeof(InfrastructureExtensions));
      var callingAssembly = Assembly.GetCallingAssembly();
      
      if (coreAssembly != null)
      {
        assemblies.Add(coreAssembly);
      }
      if (infrastructureAssembly != null)
      {
        assemblies.Add(infrastructureAssembly);
      }
      assemblies.Add(callingAssembly);
      
      services.AddMediatR(assemblies.ToArray());
      
      services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
      services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

      services.AddScoped<MockDatabase>();

      return services;
    }
  }
}
