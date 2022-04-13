using System;
using System.Collections.Generic;

namespace Sentaku.SharedKernel.Models.VotingSessionResults;

public class VoteSessionResultsDto
{
  public string Agenda { get; set; }
  public DateTime? ActivatedOn { get; set; }

  public IEnumerable<QuestionWithResultsDto> Questions { get; set; }
}
