using System;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Project.ApplicationCore.Aggregates;

namespace Project.Infrastructure.Data
{
  public class AppUser : IdentityUser
  {
    public AppUser() : base() { }

    public AppUser(string userName) : base(userName) { }

    public string Name { get; private set; }
    public string Surname { get; private set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public bool Verified { get; private set; }
    public AppFile? ProfileImage { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTime Birthday { get; private set; }

    public int LanguageId { get; set; }

    public void UpdateProfileInfo(string name, string surname)
    {

      Name = Guard.Against.NullOrWhiteSpace(name, nameof(name)); ;
      Surname = Guard.Against.NullOrWhiteSpace(surname, nameof(surname));
    }

    public void UpdateVerification(bool verified)
    {
      Verified = verified;
    }

    public void UpdateBirthday(DateTime birthday)
    {
      Birthday = Guard.Against.OutOfSQLDateRange(birthday, nameof(birthday));
    }
  }
}
