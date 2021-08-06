using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Project.SharedKernel.Constants;

namespace Project.WebMVC.Authorization.PermissionsAuthorization
{
  public class PermissionsPolicyProvider: IAuthorizationPolicyProvider
  {
    public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
    
    public PermissionsPolicyProvider(IOptions<AuthorizationOptions> options)
    {
      FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
    public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
      if (policyName.StartsWith("RequirePermissions", StringComparison.OrdinalIgnoreCase))
      {
        var requirePermissions = Enum.Parse<Permissions>(policyName.Split(":")[1]);
        
        var policy = new AuthorizationPolicyBuilder();
        policy.AddRequirements(new PermissionsRequirement(requirePermissions));
        return Task.FromResult(policy.Build());
      }

      return FallbackPolicyProvider.GetPolicyAsync(policyName);
    }
  }
}
