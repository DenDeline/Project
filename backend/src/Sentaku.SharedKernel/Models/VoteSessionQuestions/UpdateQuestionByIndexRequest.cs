using System.ComponentModel.DataAnnotations;

namespace Sentaku.SharedKernel.Models.VoteSessionQuestions;

public class UpdateQuestionByIndexRequest
{
  [Required]
  public string Summary { get; set; }
  
  [Required]
  public string Desciption { get; set; }
}
