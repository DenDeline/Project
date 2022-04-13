using System.Collections.Generic;

namespace Sentaku.SharedKernel.Models.VotingSessionResults;

public class QuestionWithResultsDto
{
  public string Summary { get; set; }
  public string Details { get; set; }
  
  public IEnumerable<ResultsDto> Results { get; set; }
}
