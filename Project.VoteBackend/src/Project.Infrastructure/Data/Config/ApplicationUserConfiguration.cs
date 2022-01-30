using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Project.Infrastructure.Data.Config
{
  public class ApplicationUserConfiguration: IEntityTypeConfiguration<ApplicationUser>
  {
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
      builder
        .HasOne(user => user.ProfileImage)
        .WithOne()
        .HasForeignKey<ApplicationUser>(user => user.ProfileImageId);
    }
  }
}
