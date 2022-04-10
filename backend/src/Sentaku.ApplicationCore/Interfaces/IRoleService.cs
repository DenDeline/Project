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
    Task<Result<VotingManager>> CreateVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default);

    Task<Result> ArchiveVotingManagerByIdAsync(
      Guid managerId,
      CancellationToken cancellationToken = default);

    Task<Result> RestoreVotingManagerByUsernameAsync(
      Guid managerId,
      CancellationToken cancellationToken = default);
    
    Task<Result> DeleteVotingManagerByUsernameAsync(
      Guid managerId,
      CancellationToken cancellationToken = default);
  }
}
