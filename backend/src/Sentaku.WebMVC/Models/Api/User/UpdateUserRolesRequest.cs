using System.Collections.Generic;

namespace Sentaku.WebMVC.Models.Api.User
{
  public class UpdateUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
