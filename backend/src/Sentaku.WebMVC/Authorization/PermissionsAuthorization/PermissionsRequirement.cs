using Microsoft.AspNetCore.Authorization;
using Project.SharedKernel.Constants;

namespace Project.WebMVC.Authorization.PermissionsAuthorization
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
