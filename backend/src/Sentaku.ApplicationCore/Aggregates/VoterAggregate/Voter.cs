using System;
using System.Collections.Generic;
using Sentaku.ApplicationCore.ValueObjects;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoterAggregate
{
  public class Voter : IdentityEntity<Guid>, IAggregateRoot
  {
    private Voter() {}

    private readonly List<JoinedVotersSessions> _joinedSessions = new();
    public IReadOnlyList<JoinedVotersSessions> JoinedSessions => _joinedSessions.AsReadOnly();

    public Voter(string identityId): base(identityId)
    {
      
    }
  }
}
