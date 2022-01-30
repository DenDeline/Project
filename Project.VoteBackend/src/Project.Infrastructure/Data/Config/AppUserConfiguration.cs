using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Infrastructure.Data.Config
{
  public class ApplicationUserConfiguration : IEntityTypeConfiguration<AppUser>
  {
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
      builder
        .HasOne(user => user.ProfileImage)
        .WithOne()
        .HasForeignKey<AppUser>(user => user.ProfileImageId);
    }
  }
}
