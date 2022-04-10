using System.Collections.Generic;

namespace Sentaku.SharedKernel.Models.User;

public class DeleteUserRolesRequest
{
  public IReadOnlyList<string> Roles { get; set; }
}
