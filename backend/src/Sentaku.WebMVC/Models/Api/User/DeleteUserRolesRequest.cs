using System.Collections.Generic;

namespace Sentaku.WebMVC.Models.Api.User
{
  public class DeleteUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
