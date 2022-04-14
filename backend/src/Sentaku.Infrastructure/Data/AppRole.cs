using Microsoft.AspNetCore.Identity;
using Sentaku.SharedKernel.Enums;

namespace Sentaku.Infrastructure.Data
{
  public class AppRole : IdentityRole
  {
    public AppRole() : base()
    {

    }

    public AppRole(string roleName) : base(roleName)
    {

    }

    public int Position { get; set; }
    public Permissions Permissions { get; set; }
  }
}
