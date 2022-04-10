using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Sentaku.SharedKernel.Constants;

namespace Sentaku.ApplicationCore.Interfaces
{
  public interface IPermissionsService
  {
    Task<Result<Permissions>> GetPermissionsAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default);
    Task<IdentityResult> ValidatePermissionsAsync(ClaimsPrincipal claimsPrincipal, Permissions requirePermissions, CancellationToken cancellationToken = default);
  }
}
