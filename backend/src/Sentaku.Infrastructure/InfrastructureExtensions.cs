using Microsoft.Extensions.DependencyInjection;
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

      services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

      services.AddScoped<MockDatabase>();

      return services;
    }
  }
}
