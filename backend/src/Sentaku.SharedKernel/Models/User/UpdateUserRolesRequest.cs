using System.Collections.Generic;

namespace Sentaku.SharedKernel.Models.User;

public class UpdateUserRolesRequest
{
  public IReadOnlyList<string> Roles { get; set; }
}