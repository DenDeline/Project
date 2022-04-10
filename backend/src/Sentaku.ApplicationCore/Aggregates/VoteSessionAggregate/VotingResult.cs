using System;
using Ardalis.GuardClauses;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;

public class VotingResult
{
  public Question Question { get; }
  public Guid QuestionId { get; }

  public VoteTypes Type { get; }

  public int Count { get; }

  private VotingResult() {}
  
  public VotingResult(Question question, VoteTypes type, int count)
  {
    Question = Guard.Against.Null(question);
    Type = Guard.Against.Null(type);
    Count = count;
  }
}
