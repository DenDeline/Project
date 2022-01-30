using Microsoft.AspNetCore.Identity;
using Project.SharedKernel.Constants;
using Project.SharedKernel.Interfaces;

namespace Project.Infrastructure.Data
{
  public class ApplicationRole: IdentityRole, IAggregateRoot
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
