using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sentaku.Infrastructure.Data.Config
{
  public class ApplicationUserConfiguration : IEntityTypeConfiguration<AppUser>
  {
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
      builder.Property(_ => _.Id)
        .HasConversion<Guid>();

      builder.Property(_ => _.Name)
        .HasMaxLength(64)
        .IsRequired();

      builder.Property(_ => _.Surname)
        .HasMaxLength(64)
        .IsRequired();

      builder
        .HasOne(user => user.ProfileImage)
        .WithOne()
        .HasForeignKey<AppUser>(user => user.ProfileImageId);
    }
  }
}
