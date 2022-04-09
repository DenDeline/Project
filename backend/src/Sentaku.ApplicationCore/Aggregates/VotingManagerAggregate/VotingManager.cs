using System;
using Ardalis.GuardClauses;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Events;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate
{
  public class VotingManager : IdentityEntity<Guid>, IAggregateRoot
  {
    private VotingManager() {}
    
    public VotingManager(string identityId): base(identityId)
    {
      
    }

    public Guid CreateVoteSession(
      string agenda,
      DateTimeOffset startDate)
    {
      Guard.Against.NullOrWhiteSpace(agenda, nameof(agenda));
      Guard.Against.OutOfSQLDateRange(startDate.UtcDateTime, nameof(startDate));

      var voteSessionId = Guid.NewGuid();
      
      var domainEvent = new CreateVoteSessionEvent(this, voteSessionId, agenda, startDate);
      
      Events.Add(domainEvent);

      return voteSessionId;
    }
  }
}
