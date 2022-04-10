using System;

namespace Sentaku.SharedKernel.Models.VotingManagers;

public class GetVotingManagerByUsernameResponse
{
  public Guid Id { get; set; }
  public UserDto Identity { get; set; }
  public bool IsArchived { get; set; }
  public DateTime? ArchivedOn { get; set; }
}
