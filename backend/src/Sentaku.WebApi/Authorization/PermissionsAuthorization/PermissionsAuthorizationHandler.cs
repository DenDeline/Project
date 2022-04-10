using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Sentaku.ApplicationCore.Interfaces;

namespace Sentaku.WebApi.Authorization.PermissionsAuthorization
{
  internal class PermissionsAuthorizationHandler : AuthorizationHandler<PermissionsRequirement>
  {
    private readonly IPermissionsService _permissionsService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PermissionsAuthorizationHandler(
      IPermissionsService permissionsService,
      IHttpContextAccessor httpContextAccessor)
    {
      _permissionsService = permissionsService;
      _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionsRequirement requirement)
    {
      if (_httpContextAccessor.HttpContext is null)
        return;

      var permissionsResult = await _permissionsService.ValidatePermissionsAsync(context.User, requirement.Permissions, _httpContextAccessor.HttpContext.RequestAborted);
      
      if (permissionsResult.Succeeded)
        context.Succeed(requirement);
      
      foreach (var permissionsResultError in permissionsResult.Errors)
      {
        context.Fail(new AuthorizationFailureReason(this, permissionsResultError.Description));
      }
    }
  }
}
