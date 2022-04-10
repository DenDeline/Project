using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Sentaku.Infrastructure.Data.Config;

public class ApplicationRoleConfig : IEntityTypeConfiguration<AppRole>
{
  public void Configure(EntityTypeBuilder<AppRole> builder)
  {
    builder.Property(_ => _.Id)
      .HasConversion<Guid>();
    
    builder
      .Property(_ => _.Position)
      .IsRequired();

    builder.HasCheckConstraint("CK_Position", "[Position] > 0");
  }
}
