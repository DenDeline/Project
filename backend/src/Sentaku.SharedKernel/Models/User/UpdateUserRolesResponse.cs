using System.Collections.Generic;

namespace Sentaku.WebApi.Models.User;

public class UpdateUserRolesResponse
{
  public IEnumerable<string> Roles { get; set; }
}