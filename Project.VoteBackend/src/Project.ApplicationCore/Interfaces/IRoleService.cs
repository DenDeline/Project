using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;

namespace Project.ApplicationCore.Interfaces
{
  public interface IRoleService
  {
    Task<Result<IReadOnlyList<string>>> UpdateRolesByUsernameAsync(
      string currentUsername,  
      string updatingUsername, 
      IReadOnlyList<string> updatingRoles,
      CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<string>>> DeleteUserRolesByUsernameAsync(
      string currentUsername, 
      string updatingUsername,
      CancellationToken cancellationToken = default);
  }
}
