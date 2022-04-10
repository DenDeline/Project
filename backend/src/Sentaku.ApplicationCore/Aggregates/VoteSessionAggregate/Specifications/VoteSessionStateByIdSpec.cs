using System;
using Ardalis.Specification;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionStateByIdSpec: Specification<VoteSession, SessionState>
{
  public VoteSessionStateByIdSpec(Guid sessionId)
  {
    Query
      .Select(_ => _.State)
      .Where(_ => _.Id == sessionId);
  }
}
