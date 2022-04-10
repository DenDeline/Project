using System;
using System.Collections.Generic;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoterAggregate
{
  public class Voter : IdentityEntity<Guid>, IAggregateRoot
  {
    private Voter() {}

    private readonly List<VoteSession> _sessions = new();
    public IReadOnlyList<VoteSession> Sessions => _sessions.AsReadOnly();

    public Voter(string identityId): base(identityId)
    {
      
    }
  }
}
