using System.Collections.Generic;

namespace Project.WebMVC.Models.Api.User
{
  public class UpdateUserRolesResponse
  {
    public IEnumerable<string> Roles { get; set; }
  }
}
