using System.Collections.Generic;

namespace Project.WebMVC.Models.Api.Users
{
  public class DeleteUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
