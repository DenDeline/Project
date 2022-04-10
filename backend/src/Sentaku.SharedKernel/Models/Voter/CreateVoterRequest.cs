using System.ComponentModel.DataAnnotations;

namespace Sentaku.SharedKernel.Models.Voter;

public class CreateVoterRequest
{
  [Required]
  public string Username { get; set; }
}
