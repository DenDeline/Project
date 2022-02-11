using System.Collections.Generic;

namespace Sentaku.WebApi.Models.User;

public class DeleteUserRolesRequest
{
  public IReadOnlyList<string> Roles { get; set; }
}
