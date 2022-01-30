using Microsoft.Extensions.DependencyInjection;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;
using Project.Infrastructure.Services;
using Project.SharedKernel.Interfaces;

namespace Project.Infrastructure
{
  public static class InfrastructureExtensions
  {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
      services.AddSingleton<IIdentityTokenClaimService, IdentityTokenClaimService>();
      services.AddScoped<IRoleService, RoleService>();
      services.AddScoped<IPermissionsService, PermissionsService>();
      services.AddScoped<IUserVerificationService, UserVerificationService>();

      services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

      services.AddScoped<MockDatabase>(); 
      
      return services;
    }
  }
}
