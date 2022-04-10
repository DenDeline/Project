using System.ComponentModel.DataAnnotations;

namespace Sentaku.WebApi.Models.VotingManagers;

public class CreateVoterRequest
{
  [Required]
  public string Username { get; set; }
}
