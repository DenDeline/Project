using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionJoinVotersBySessionIdSpec: Specification<VoteSession, IEnumerable<Guid>>
{
  public VoteSessionJoinVotersBySessionIdSpec(Guid sessionId)
  {
    Query
      .Select(_ => _.JoinedVoters.Select(_ => _.VoterId))
      .Include(_ => _.JoinedVoters)
      .Where(_ => _.Id == sessionId);
  }
}
