using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;

namespace Sentaku.ApplicationCore.Interfaces
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

    Task<Result<VotingManager>> CreateVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default);

    Task<Result> ArchiveVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default);

    Task<Result> RestoreVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default);
    
    Task<Result> DeleteVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default);
  }
}
