using System.Collections.Generic;

namespace Sentaku.WebApi.Models.Api.User
{
  public class UpdateUserRolesRequest
  {
    public IReadOnlyList<string> Roles { get; set; }
  }
}
