using System.Linq;
using System.Reflection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Sentaku.WebApi.Authorization.PermissionsAuthorization;

public class PermissionsOperationFilter: IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    var requiredPermissions = context
      .MethodInfo
      .DeclaringType?
      .GetCustomAttributes<RequirePermissionsAttribute>()
      .Select(_ => _.Permissions)
      .ToList();
    
    if (requiredPermissions is null || !requiredPermissions.Any())
      return;

    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
    
    // var permissions = requiredPermissions.Aggregate((current, rolePermission) => current | rolePermission);
    //
    // var permissionsScheme = new OpenApiSecurityScheme
    // {
    //   Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "permissions" }
    // };
    //
    // var permissionsList = Enum.GetValues<Permissions>()
    //   .Where(_ => _.HasFlag(permissions))
    //   .Select(_ => _.GetDisplayName());
    //
    // foreach(var permission in permissionsList)
    //   operation.Tags.Add(new () { Name = permission });
  }
}
