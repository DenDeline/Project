using System;
using System.Linq;
using Ardalis.Specification;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.SharedKernel.Models.VotingSessionResults;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionResultsByIdSpec: Specification<VoteSession, VoteSessionResultsDto>
{
  public VoteSessionResultsByIdSpec(Guid sessionId)
  {
    Query
      .Select(session => new VoteSessionResultsDto
      {
        Agenda = session.Agenda,
        ActivatedOn = session.ActivatedOn,
        Questions = session.Questions
          .Select(question => new QuestionWithResultsDto 
          {
            Summary = question.Summary,
            Details = question.Description,
            Results = question.Results
              .Select(_ => new ResultsDto
              {
                Type = ((VoteTypes) _.Type).Name, 
                Count = _.Count
              }) 
          })
      })
      .Include(_ => _.Questions)
        .ThenInclude(_ => _.Results)
      .Where(_ => _.Id == sessionId);
  }
}
