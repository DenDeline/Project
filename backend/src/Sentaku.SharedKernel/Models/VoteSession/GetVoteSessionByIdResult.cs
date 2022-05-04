using System;

namespace Sentaku.SharedKernel.Models.VoteSession;

public record GetVoteSessionByIdResult(
  Guid Id, 
  Guid? VotingManagerId,
  string State, 
  string Agenda,
  DateTimeOffset StartDate,
  DateTime CreatedOn, 
  DateTime? ActivatedOn,
  DateTime? ClosedOn,
  DateTime? ResultsApprovedOn,
  int QuestionCount);
