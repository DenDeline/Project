using System;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionByIdIncludeQuestionsSpec: Specification<VoteSession>, ISingleResultSpecification
{
  public VoteSessionByIdIncludeQuestionsSpec(Guid sessionId)
  {
    Query
      .Include(_ => _.Questions)
      .Where(_ => _.Id == sessionId);
  }
}
