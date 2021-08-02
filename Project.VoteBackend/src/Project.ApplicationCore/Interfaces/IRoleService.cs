using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Project.ApplicationCore.Entities;

namespace Project.ApplicationCore.Interfaces
{
  public interface IRoleService
  { 
    Task<Result<IReadOnlyList<string>>> AddRolesByUserNameAsync(
      AppUser currentUser, 
      string username, 
      IReadOnlyList<string> updatingRoles,
      CancellationToken cts);

    Task<Result<IReadOnlyList<string>>> RemoveRolesByUserNameAsync(
      AppUser currentUser,
      string username,
      IReadOnlyList<string> updatingRoles,
      CancellationToken cts = new CancellationToken());
  }
}
