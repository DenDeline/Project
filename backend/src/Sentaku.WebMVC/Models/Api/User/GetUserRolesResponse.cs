using System.Collections.Generic;

namespace Sentaku.WebMVC.Models.Api.User
{
  public class GetUserRolesResponse
  {
    public IEnumerable<string> Roles { get; set; }
  }
}
