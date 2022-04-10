using System;

namespace Sentaku.WebApi.Models.VotingManagers;

public class GetVoterByIdResponse
{
  public Guid Id { get; set; }
  public UserDto Identity { get; set; }
  public bool IsArchived { get; set; }
  public DateTime? ArchivedOn { get; set; }
}
