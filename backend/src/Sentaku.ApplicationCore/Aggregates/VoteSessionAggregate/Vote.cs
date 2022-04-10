using System;
using Ardalis.GuardClauses;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;

public class Vote
{
  public Voter Voter { get; }
  public Guid VoterId { get; }
  
  public VoteTypes Type { get; }
  
  public Question Question { get; }
  public Guid QuestionId { get; }
  
  private Vote() {}

  public Vote(Question question, Voter voter, VoteTypes type)
  {
    Question = Guard.Against.Null(question, nameof(question));
    Voter = Guard.Against.Null(voter, nameof(voter));
    Type = Guard.Against.Null(type, nameof(type));
  }
}
