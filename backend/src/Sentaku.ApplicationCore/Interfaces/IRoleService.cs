using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.SharedKernel;

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

    Task<Result> RestoreVotingManagerByIdAsync(
      Guid managerId,
      CancellationToken cancellationToken = default);
    
    Task<Result> DeleteVotingManagerByIdAsync(
      Guid managerId,
      CancellationToken cancellationToken = default);
    
    Task<Result<Voter>> CreateVoterByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default);

    Task<Result> ArchiveVoterByIdAsync(
      Guid voterId,
      CancellationToken cancellationToken = default);

    Task<Result> RestoreVoterByIdAsync(
      Guid voterId,
      CancellationToken cancellationToken = default);
    
    Task<Result> DeleteVoterByIdAsync(
      Guid voterId,
      CancellationToken cancellationToken = default);
  }
}
