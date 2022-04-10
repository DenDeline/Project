using System;
using System.Linq;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class QuestionByVoteSessionIdAndIndexSpec: Specification<VoteSession, Question>
{
  public QuestionByVoteSessionIdAndIndexSpec(Guid sessionId, int questionIndex)
  {
    Query
      .Select(_ => _.Questions.SingleOrDefault(_ => _.Index == questionIndex))
      .Include(_ => _.Questions)
      .Where(_ => _.Id == sessionId);
  }
}
