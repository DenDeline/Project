using System.ComponentModel.DataAnnotations;

namespace Sentaku.WebApi.Models.VotingManagers;

public class CreateVotingManagerRequest
{
  [Required]
  public string Username { get; set; }
}
