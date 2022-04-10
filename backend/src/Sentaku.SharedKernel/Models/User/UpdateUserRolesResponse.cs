using System.Collections.Generic;

namespace Sentaku.SharedKernel.Models.User;

public class UpdateUserRolesResponse
{
  public IEnumerable<string> Roles { get; set; }
}