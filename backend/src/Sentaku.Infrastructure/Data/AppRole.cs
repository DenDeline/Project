using Microsoft.AspNetCore.Identity;
using Project.SharedKernel.Constants;
using Project.SharedKernel.Interfaces;

namespace Project.Infrastructure.Data
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
