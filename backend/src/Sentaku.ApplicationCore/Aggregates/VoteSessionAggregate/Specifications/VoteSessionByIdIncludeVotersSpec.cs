using System;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionByIdIncludeVotersSpec: Specification<VoteSession>, ISingleResultSpecification
{
  public VoteSessionByIdIncludeVotersSpec(Guid sessionId)
  {
    Query
      .Include(_ => _.Voters)
      .Where(_ => _.Id == sessionId);
  }
}
