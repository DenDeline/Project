using System;
using Ardalis.GuardClauses;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate
{
  public class VoteSession : BaseEntity<Guid>, IAggregateRoot
  {
    public DateTime CreatedOn { get; }
    public VotingManager VotingManager { get; }
    public Guid VotingManagerId { get; }
    
    public string Agenda { get; }
    
    public DateTimeOffset StartDate { get; }
    
    public SessionState State { get; }
    
    private VoteSession() {}

    public VoteSession(VotingManager votingManager, string agenda)
    {
      Guard.Against.Null(votingManager, nameof(votingManager));
      Guard.Against.NullOrWhiteSpace(agenda, nameof(agenda));
      
      Agenda = agenda;
      VotingManager = votingManager;
      
      CreatedOn = DateTime.UtcNow;
      StartDate = DateTimeOffset.UtcNow;
      State = SessionState.Active;
    }
    
    public VoteSession(VotingManager votingManager, string agenda, DateTimeOffset startDate)
    {
      Guard.Against.Null(votingManager, nameof(votingManager));
      Guard.Against.OutOfSQLDateRange(startDate.UtcDateTime, nameof(startDate));
      Guard.Against.NullOrWhiteSpace(agenda, nameof(agenda));

      StartDate = startDate;
      Agenda = agenda;
      VotingManager = votingManager;
      
      CreatedOn = DateTime.UtcNow;
      State = SessionState.Pending;
    }
  }
}
