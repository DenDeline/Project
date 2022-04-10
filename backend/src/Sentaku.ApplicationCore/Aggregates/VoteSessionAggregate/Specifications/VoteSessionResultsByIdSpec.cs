using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using Sentaku.SharedKernel.Models.VoteSession;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionResultsByIdSpec: Specification<VoteSession, IEnumerable<VoteSessionResultsDto>>
{
  public VoteSessionResultsByIdSpec(Guid sessionId)
  {
    Query
      .Select(session => session.Questions.Select(question =>  new VoteSessionResultsDto
      {
        Index = question.Index,
        Results = question.Results.Select(_ => new VoteCountDto{ Type = _.Type, Count = _.Count })
      }))
      .Include(_ => _.Questions)
      .ThenInclude(_ => _.Results)
      .Where(_ => _.Id == sessionId);
  }
}
