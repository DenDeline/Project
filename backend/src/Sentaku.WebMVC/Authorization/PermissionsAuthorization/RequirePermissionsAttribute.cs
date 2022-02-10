using System;
using Microsoft.AspNetCore.Authorization;
using Sentaku.SharedKernel.Constants;

namespace Sentaku.WebMVC.Authorization.PermissionsAuthorization
{
  internal class RequirePermissionsAttribute : AuthorizeAttribute
  {
    private const string PolicyPrefix = "RequirePermissions";

    public RequirePermissionsAttribute(Permissions permissions) => Permissions = permissions;

    public Permissions Permissions
    {
      get
      {
        if (Enum.TryParse(Policy?.Split(':')[1], out Permissions permissions))
        {
          return permissions;
        }

        return default;
      }
      set
      {
        Policy = $"{PolicyPrefix}:{((int)value).ToString()}";
      }
    }
  }
}
