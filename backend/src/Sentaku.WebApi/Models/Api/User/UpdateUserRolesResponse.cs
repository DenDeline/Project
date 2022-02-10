using System.Collections.Generic;

namespace Sentaku.WebApi.Models.Api.User
{
  public class UpdateUserRolesResponse
  {
    public IEnumerable<string> Roles { get; set; }
  }
}
