using System;
using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Identity;
using Project.ApplicationCore.Aggregates;
using Project.SharedKernel.Interfaces;

namespace Project.Infrastructure.Data
{
  public class ApplicationUser: IdentityUser, IAggregateRoot
  {
    public ApplicationUser() : base()
    {
      CreatedAt = DateTime.UtcNow;
    }

    public ApplicationUser(string userName) : base(userName)
    {
      CreatedAt = DateTime.UtcNow;
    }

    public string Name { get; private set; }
    public string Surname { get; private set; }
    public DateTime CreatedAt { get; }
    public bool Verified { get; private set; }
    public AppFile? ProfileImage { get; set; }
    public string? ProfileImageId { get; set; }
    public DateTime Birthday { get; private set; }
    
    public int LanguageId { get; set; }

    public void UpdateProfileInfo(string name, string surname)
    {
      
      Name = Guard.Against.NullOrWhiteSpace(name, nameof(name));;
      Surname = Guard.Against.NullOrWhiteSpace(surname, nameof(surname));
    }

    public void UpdateVerification(bool verified)
    {
      Verified = verified;
    }

    public void UpdateBirthday(DateTime birthday)
    {
      Birthday = Guard.Against.Default(birthday, nameof(birthday));
    }
  }
}
