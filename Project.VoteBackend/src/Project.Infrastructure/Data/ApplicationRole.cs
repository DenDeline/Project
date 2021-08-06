using Microsoft.AspNetCore.Identity;
using Project.SharedKernel.Constants;

namespace Project.Infrastructure.Data
{
  public class ApplicationRole: IdentityRole
  {
    public ApplicationRole(): base()
    {
      
    }
    
    public ApplicationRole(string roleName) : base(roleName)
    {
      
    }
    
    public int Position { get; set; }
    public Permissions Permissions { get; set; }
  }
}
