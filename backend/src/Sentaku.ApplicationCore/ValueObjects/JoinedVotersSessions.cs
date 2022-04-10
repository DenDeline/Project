using System;
using Ardalis.GuardClauses;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;

namespace Sentaku.ApplicationCore.ValueObjects;

public class JoinedVotersSessions
{
  public Voter Voter { get; }
  public Guid VoterId { get; }
  
  public VoteSession VoteSession { get; }
  public Guid VoteSessionId { get; }
  
  public bool IsVoted { get; set; }
  public bool IsFavorite { get; set; }

  private JoinedVotersSessions() { }
  
  public JoinedVotersSessions(Voter voter, VoteSession session)
  {
    Voter = Guard.Against.Null(voter, nameof(voter));
    VoteSession = Guard.Against.Null(session, nameof(session));
  }
}
