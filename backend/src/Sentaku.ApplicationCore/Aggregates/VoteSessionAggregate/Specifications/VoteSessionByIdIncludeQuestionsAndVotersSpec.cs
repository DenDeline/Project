using System;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionByIdIncludeQuestionsAndVotersSpec: Specification<VoteSession>, ISingleResultSpecification
{
  public VoteSessionByIdIncludeQuestionsAndVotersSpec(Guid sessionId)
  {
    Query
      .Include(_ => _.Questions)
      .Include(_ => _.JoinedVoters)
      .Where(_ => _.Id == sessionId);
  }
}
