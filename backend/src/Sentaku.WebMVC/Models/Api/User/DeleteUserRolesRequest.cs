using System.Collections.Generic;

namespace Project.WebMVC.Models.Api.User
{
  public class DeleteUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
