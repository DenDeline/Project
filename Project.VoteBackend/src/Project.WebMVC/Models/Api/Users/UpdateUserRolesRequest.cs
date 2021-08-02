using System.Collections.Generic;

namespace Project.WebMVC.Models.Api.Users
{
  public class UpdateUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
