using System.ComponentModel.DataAnnotations;

namespace Sentaku.SharedKernel.Models.VotingManagers;

public class CreateVotingManagerRequest
{
  [Required]
  public string Username { get; set; }
}
