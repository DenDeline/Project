using System.Collections.Generic;

namespace Sentaku.WebMVC.Models.Api.User
{
  public class UpdateUserRolesResponse
  {
    public IEnumerable<string> Roles { get; set; }
  }
}
