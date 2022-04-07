using System;
using Sentaku.SharedKernel;

namespace Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Events;

public class CreateVoteSessionEvent: BaseDomainEvent
{
  public VotingManager VotingManager { get; }
  public DateTimeOffset StartDate { get; }
  public string Agenda { get; }

  public CreateVoteSessionEvent(
    VotingManager votingManager,
    string agenda,
    DateTimeOffset startDate)
  {
    VotingManager = votingManager;
    StartDate = startDate;
    Agenda = agenda;
  }
}
