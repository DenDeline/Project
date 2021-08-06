using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;

namespace Project.ApplicationCore.Interfaces
{
  public interface IRoleService
  { 
    Task<Result<IReadOnlyList<string>>> AddRolesByUserNameAsync(
      string currentUserName, 
      string updatingUserName, 
      IReadOnlyList<string> updatingRoles,
      CancellationToken cts);

    Task<Result<IReadOnlyList<string>>> RemoveRolesByUserNameAsync(
      string currentUserName, 
      string updatingUserName, 
      IReadOnlyList<string> updatingRoles,
      CancellationToken cts = new CancellationToken());
  }
}
