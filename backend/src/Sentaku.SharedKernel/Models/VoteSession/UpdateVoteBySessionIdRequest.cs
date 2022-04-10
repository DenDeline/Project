using System.ComponentModel.DataAnnotations;

namespace Sentaku.SharedKernel.Models.VoteSession;

public class UpdateVoteBySessionIdRequest
{
  [Required]
  public int[] Votes { get; set; }
}
