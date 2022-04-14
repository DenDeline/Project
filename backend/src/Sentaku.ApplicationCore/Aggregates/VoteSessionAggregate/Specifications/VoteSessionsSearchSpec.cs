using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Ardalis.Specification;
using Sentaku.SharedKernel.Enums;
using Sentaku.SharedKernel.Models;
using Sentaku.SharedKernel.Models.VoteSession;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteSessionsSearchSpec: Specification<VoteSession>
{
  private readonly Dictionary<VoteSessionSortingColumns, Expression<Func<VoteSession, object?>>> _sortingFields = new()
  {
    [VoteSessionSortingColumns.CreatedOn] = session => session.CreatedOn,
    [VoteSessionSortingColumns.VotingManager] = session => session.VotingManagerId
  };

  public VoteSessionsSearchSpec(SortingDto<VoteSessionSortingColumns> sorting)
  {
    var sortingField = _sortingFields.GetValueOrDefault(sorting.ColumnType, session => session.CreatedOn);
    
    if (sorting.Order == SortingOrder.Ascending)
      Query.OrderBy(sortingField);
    else
      Query.OrderByDescending(sortingField);

    Query
      .Include(_ => _.VotingManager);
    
  }
}
