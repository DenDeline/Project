using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Enums;

namespace Sentaku.Infrastructure.Services
{
  public class PermissionsService : IPermissionsService
  {
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<AppUser> _userManager;

    public PermissionsService(AppDbContext appDbContext, UserManager<AppUser> userManager)
    {
      _appDbContext = appDbContext;
      _userManager = userManager;
    }

    public async Task<IdentityResult> ValidatePermissionsAsync(ClaimsPrincipal claimsPrincipal, Permissions requirePermissions, CancellationToken cancellationToken = default)
    {
      var userId = _userManager.GetUserId(claimsPrincipal);
      
      // TODO: add validation errors
      if (userId is null)
        return IdentityResult.Failed();

      if (requirePermissions == Permissions.None)
        return IdentityResult.Success;

      var permissionsQuery = from userRole in _appDbContext.UserRoles
          join role in _appDbContext.Roles on userRole.RoleId equals role.Id
          where userRole.UserId.Equals(userId)
          select role.Permissions;

      var permissionsResult = await permissionsQuery.ToListAsync(cancellationToken);

      if (!permissionsResult.Any())
        return IdentityResult.Failed();
      
      var permissions = permissionsResult.Aggregate((current, rolePermission) => current | rolePermission);

      if ((permissions & Permissions.Administrator) == Permissions.Administrator)
        return IdentityResult.Success;

      return (permissions & requirePermissions) == requirePermissions
        ? IdentityResult.Success
        : IdentityResult.Failed();
    }

    public async Task<Result<Permissions>> GetPermissionsAsync(
      ClaimsPrincipal claimsPrincipal,
      CancellationToken cancellationToken = default)
    {
      var userId = _userManager.GetUserId(claimsPrincipal);
      
      // TODO: add validation errors
      if (userId is null)
        return Result<Permissions>.NotFound();

      var permissionsQuery = from userRole in _appDbContext.UserRoles
        join role in _appDbContext.Roles on userRole.RoleId equals role.Id
        where userRole.UserId.Equals(userId)
        select role.Permissions;

      var permissionsResult = await permissionsQuery.ToListAsync(cancellationToken);

      if (!permissionsResult.Any())
        return Permissions.None;
      
      var permissions = permissionsResult.Aggregate((current, rolePermission) => current | rolePermission);

      return (permissions & Permissions.Administrator) == Permissions.Administrator ? Permissions.All : permissions;
    }
  }
}
