using System.Collections.Generic;

namespace Project.WebMVC.Models.Api.User
{
  public class UpdateUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
