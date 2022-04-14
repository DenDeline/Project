using Ardalis.SmartEnum;

namespace Sentaku.SharedKernel.Enums
{
  public abstract class RolesEnum : SmartEnum<RolesEnum>
  {
    public static readonly RolesEnum Administrator = new AdministratorRole();
    public static readonly RolesEnum LeadManager = new LeadManagerRole();
    public static readonly RolesEnum RepresentativeAuthority = new RepresentativeAuthorityRole();
    public static readonly RolesEnum Authority = new AuthorityRole();
    
    public abstract int Position { get; }
    public abstract string DisplayName { get; }

    public bool CanModify(RolesEnum role) => Position > role.Position;

    protected RolesEnum(string name, int value) : base(name, value) { }

    private sealed class AdministratorRole : RolesEnum
    {
      public AdministratorRole() : base("Administrator", 1) { }
      public override int Position => 1;
      public override string DisplayName => "Administrator";
    }
    
    private sealed class LeadManagerRole : RolesEnum
    {
      public LeadManagerRole() : base("LeadManager", 2) { }
      public override int Position => 2;
      public override string DisplayName => "Lead Manager";
    }

    private sealed class RepresentativeAuthorityRole : RolesEnum
    {
      public RepresentativeAuthorityRole() : base("RepresentativeAuthority", 3)
      {
      }

      public override int Position => 3;
      public override string DisplayName => "Representative Authority";
    }
    
    private sealed class AuthorityRole : RolesEnum
    {
      public AuthorityRole() : base("Authority", 4)
      {
      }

      public override int Position => 4;
      public override string DisplayName => "Authority";
    }
  }

  public enum Roles
  {
    Administrator,
    LeadManager,
    RepresentativeAuthority,
    Authority
  }
}
