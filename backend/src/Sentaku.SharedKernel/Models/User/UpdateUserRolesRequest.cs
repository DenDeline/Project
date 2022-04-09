using System.Collections.Generic;

namespace Sentaku.WebApi.Models.User;

public class UpdateUserRolesRequest
{
  public IReadOnlyList<string> Roles { get; set; }
}