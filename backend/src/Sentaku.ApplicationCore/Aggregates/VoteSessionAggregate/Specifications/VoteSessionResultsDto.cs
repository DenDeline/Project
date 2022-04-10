using System.Collections.Generic;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

namespace Sentaku.SharedKernel.Models.VoteSession;

public class VoteSessionResultsDto
{
  public int Index { get; set; }
  public IEnumerable<VoteCountDto> Results { get; set; }
}
