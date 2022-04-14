using System;

namespace Sentaku.SharedKernel.Enums
{
  [Flags]
  public enum Permissions
  {
    None = 0,
    Administrator = 1 << 0,
    ManageUserRoles = 1 << 1,
    BanUsers = 1 << 2,
    VerifyUsers = 1 << 3,
    ManageVotingSessions = 1 << 4,
    ViewVotingSessions = 1 << 5,
    ViewUsers = 1 << 6,

    All = Administrator |
          ManageUserRoles |
          BanUsers |
          VerifyUsers |
          ManageVotingSessions |
          ViewVotingSessions | 
          ViewUsers
  }
}
