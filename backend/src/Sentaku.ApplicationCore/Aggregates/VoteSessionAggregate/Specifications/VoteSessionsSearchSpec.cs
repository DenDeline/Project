using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ardalis.Specification;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.SharedKernel.Enums;
using Sentaku.SharedKernel.Models;
using Sentaku.SharedKernel.Models.VoteSession;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionsSearchSpec: Specification<VoteSession, ListSessionsResult>
{
  private readonly Dictionary<VoteSessionSortingColumns, Expression<Func<VoteSession, object?>>> _sortingFields = new()
  {
    [VoteSessionSortingColumns.CreatedOn] = session => session.CreatedOn,
    [VoteSessionSortingColumns.ResultsApprovedOn] = session => session.ResultsApprovedOn
  };

  public VoteSessionsSearchSpec(SortingDto<VoteSessionSortingColumns> sorting)
  {
    var sortingField = _sortingFields.GetValueOrDefault(sorting.ColumnType, session => session.CreatedOn);

    Query.Select(_ => new ListSessionsResult(
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
    
    if (sorting.Order == SortingOrder.Ascending)
      Query.OrderBy(sortingField);
    else
      Query.OrderByDescending(sortingField);

    Query
      .Include(_ => _.VotingManager);
    
  }
}
