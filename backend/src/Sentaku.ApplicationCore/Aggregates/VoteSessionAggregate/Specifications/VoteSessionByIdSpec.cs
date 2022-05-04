using System;
using Ardalis.Specification;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.SharedKernel.Models.VoteSession;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionByIdSpec: Specification<VoteSession, GetVoteSessionByIdResult>
{
  public VoteSessionByIdSpec(Guid sessionId)
  {
    Query.Select(_ => new GetVoteSessionByIdResult(
      _.Id,
      _.VotingManagerId,
      ((SessionState)_.State).Name,
      _.Agenda,
      _.StartDate,
      _.CreatedOn,
      _.ActivatedOn,
      _.ClosedOn,
      _.ResultsApprovedOn,
      _.QuestionCount));

    Query.Where(_ => _.Id == sessionId);
  }
}
