using Microsoft.AspNetCore.Authorization;
using Sentaku.SharedKernel.Constants;

namespace Sentaku.WebApi.Authorization.PermissionsAuthorization
{
  internal class PermissionsRequirement : IAuthorizationRequirement
  {
    public Permissions Permissions { get; }

    public PermissionsRequirement(Permissions permissions)
    {
      Permissions = permissions;
    }
  }
}
