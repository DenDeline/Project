using System.ComponentModel.DataAnnotations;

namespace Sentaku.SharedKernel.Models.VoteSessionQuestions;

public class CreateQuestionByVoteSessionIdRequest
{
  [Required]
  public string Summary { get; set; }
  
  [Required]
  public string Description { get; set; }
}
