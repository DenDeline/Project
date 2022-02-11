using System.Collections.Generic;

namespace Sentaku.WebApi.Models.User;

public class GetUserRolesResponse
{
  public IEnumerable<string> Roles { get; set; }
}