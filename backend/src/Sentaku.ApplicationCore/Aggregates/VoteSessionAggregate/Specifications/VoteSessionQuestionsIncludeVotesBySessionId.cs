using System;
using System.Collections.Generic;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionQuestionsIncludeVotesBySessionId: Specification<VoteSession, IEnumerable<Question>>
{
  public VoteSessionQuestionsIncludeVotesBySessionId(Guid sessionId)
  {
    Query
      .Select(_ => _.Questions)
      .Include(_ => _.Questions)
        .ThenInclude(_ => _.Votes)
      .Where(_ => _.Id == sessionId);
  }
}
