using System.Collections.Generic;

namespace Sentaku.SharedKernel.Models.User;

public class GetUserRolesResponse
{
  public IEnumerable<string> Roles { get; set; }
}