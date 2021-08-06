using System;

namespace Project.SharedKernel.Constants
{
  [Flags]
  public enum Permissions
  {
    None = 0,
    Administrator = 1 << 0,
    ManageUserRoles = 1 << 1,
    
    
    
    All = Administrator | ManageUserRoles
  }
}
