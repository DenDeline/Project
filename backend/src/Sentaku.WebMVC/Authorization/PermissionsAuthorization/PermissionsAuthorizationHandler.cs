using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Project.ApplicationCore.Interfaces;

namespace Project.WebMVC.Authorization.PermissionsAuthorization
{
  internal class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
  {
    private readonly IPermissionsService _permissionsService;

    public PermissionsAuthorizationHandler(IPermissionsService permissionsService)
    {
      _permissionsService = permissionsService;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
    {
      if (context.User.Identity?.Name is null)
        return;

      var permissionsResult = await _permissionsService.GetPermissionsByUsernameAsync(context.User.Identity.Name);

      if (permissionsResult.IsSuccess)
      {
        if ((permissionsResult.Value & requirement.Permissions) == requirement.Permissions)
        {
          context.Succeed(requirement);
        }
      }
    }
  }
}
