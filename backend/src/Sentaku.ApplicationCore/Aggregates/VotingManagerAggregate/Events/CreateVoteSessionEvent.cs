using System;
using Sentaku.SharedKernel;

namespace Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Events;

public class CreateVoteSessionEvent: BaseDomainEvent
{
  public VotingManager VotingManager { get; }
  public Guid VoteSessionId { get; }
  public DateTimeOffset StartDate { get; }
  public string Agenda { get; }

  public CreateVoteSessionEvent(
    VotingManager votingManager,
    Guid voteSessionId,
    string agenda,
    DateTimeOffset startDate)
  {
    VotingManager = votingManager;
    VoteSessionId = voteSessionId;
    StartDate = startDate;
    Agenda = agenda;
  }
}
