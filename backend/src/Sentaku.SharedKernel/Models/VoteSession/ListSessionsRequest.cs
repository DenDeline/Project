using Sentaku.SharedKernel.Enums;

namespace Sentaku.SharedKernel.Models.VoteSession;

public class ListSessionsRequest
{
  public SortingDto<VoteSessionSortingColumns> Sorting { get; set; } = new()
  {
    Order = SortingOrder.Ascending, ColumnType = VoteSessionSortingColumns.CreatedOn
  };
}
