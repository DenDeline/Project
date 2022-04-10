using System;
using System.Collections.Generic;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class QuestionsByVoteSessionIdSpec: Specification<VoteSession, IEnumerable<Question>>
{
  public QuestionsByVoteSessionIdSpec(Guid sessionId)
  {
    Query
      .Select(_ => _.Questions)
      .Include(_ => _.Questions)
      .Where(_ => _.Id == sessionId);
  }
}
