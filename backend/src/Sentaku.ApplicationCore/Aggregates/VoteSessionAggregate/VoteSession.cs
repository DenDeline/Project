using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Ardalis.SmartEnum.SystemTextJson;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Events;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.ApplicationCore.ValueObjects;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate
{
  public class VoteSession : BaseEntity<Guid>, IAggregateRoot
  {
    public DateTime CreatedOn { get; }
    public DateTimeOffset StartDate { get; }
    
    public DateTime? ActivatedOn { get; private set; }
    public DateTime? ClosedOn { get; private set; }
    public DateTime? ResultsApprovedOn { get; private set; }
    
    public VotingManager? VotingManager { get; }
    public Guid? VotingManagerId { get; }
    
    public string Agenda { get; }
    private readonly List<Question> _questions = new();
    public IReadOnlyList<Question> Questions => _questions.AsReadOnly();
    public int QuestionCount { get; private set; }

    private readonly List<JoinedVotersSessions> _joinedVoters = new();
    public IReadOnlyList<JoinedVotersSessions> JoinedVoters => _joinedVoters.AsReadOnly();
    
    [JsonConverter(typeof(SmartEnumNameConverter<SessionState, int>))]
    public SessionState State { get; private set; }
    
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

    public Question? AddQuestion(string summary, string description)
    {
      if (!State.CanEditSession) return null;
      
      var question = new Question(this, QuestionCount++, summary, description);
      _questions.Add(question);
      return question;
    }

    public Question? GetQuestionByIndex(int questionIndex) =>
      _questions.SingleOrDefault(_ => _.Index == questionIndex);

    public bool RemoveQuestionByIndex(int questionIndex)
    {
      if (!State.CanEditSession) return false;
      
      var requireQuestion = GetQuestionByIndex(questionIndex);
      
      if (requireQuestion is null)
        return false;
      
      _questions.Remove(requireQuestion);
      
      foreach (var question in _questions.Where(_ => _.Index > questionIndex))
        --question.Index;
      
      --QuestionCount;
      
      return true;
    }

    public bool AddVotes(Voter voter, int[] votes)
    {
      Guard.Against.Null(voter, nameof(voter));
      Guard.Against.NullOrEmpty(votes, nameof(votes));

      if (!State.Equals(SessionState.Active))
        return false;

      var joinedVoter = _joinedVoters.SingleOrDefault(_ => _.VoterId == voter.Id);
      
      if(joinedVoter is null or { IsVoted: true })
        return false;

      if (_questions.Count != votes.Length)
        return false;
      
      foreach (var question in _questions)
        question.AddVote(voter, VoteTypes.FromValue(votes[question.Index]));

      joinedVoter.IsVoted = true;
      
      return true;
    }
    
    public bool JoinVoter(Voter voter)
    {
      Guard.Against.Null(voter, nameof(voter));
      
      if (!State.Equals(SessionState.Pending))
        return false;
      
      if(_joinedVoters.Any(_ => _.VoterId == voter.Id))
        return false;
      
      _joinedVoters.Add(new JoinedVotersSessions(voter, this));

      return true;
    }

    public bool MoveInNextState()
    {
      if (State.NextState is null)
        return false;

      State.NextState
        .When(SessionState.Active)
        .Then(Activate)
        .When(SessionState.Closed)
        .Then(Close)
        .When(SessionState.ResultsApproved)
        .Then(ApproveResults);
      
      return true;
    }

    private void Activate()
    {
      State = SessionState.Active;
      ActivatedOn = DateTime.UtcNow;
    }
    
    private void Close()
    {
      State = SessionState.Closed;
      ClosedOn = DateTime.UtcNow;
      
      Events.Add(new VoteSessionClosedEvent(Id));
    }
    
    private void ApproveResults()
    {
      State = SessionState.ResultsApproved;
      ResultsApprovedOn = DateTime.UtcNow;
    }
  }
}
