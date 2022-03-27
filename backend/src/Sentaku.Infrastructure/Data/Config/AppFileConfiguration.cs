using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates;

namespace Sentaku.Infrastructure.Data.Config
{
  public class AppFileConfiguration : IEntityTypeConfiguration<AppFile>
  {
    public void Configure(EntityTypeBuilder<AppFile> builder)
    {
      builder.Property(_ => _.Id)
        .HasConversion<Guid>();
      
      builder
        .Property(_ => _.ContentType)
        .HasMaxLength(256)
        .IsRequired();

      builder
        .Property(_ => _.UntrustedName)
        .HasMaxLength(256)
        .IsRequired();

      builder
        .Property(_ => _.Content)
        .HasColumnType("varbinary(max)")
        .IsRequired();
    }
  }
}
