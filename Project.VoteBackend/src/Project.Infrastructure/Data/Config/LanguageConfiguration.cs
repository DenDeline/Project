﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.ApplicationCore.Aggregates;

namespace Project.Infrastructure.Data.Config
{
    public class LanguageConfiguration: IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(e => e.Name)
                .HasMaxLength(30)
                .IsRequired();

            builder.Property(e => e.Code)
                .HasMaxLength(5)
                .IsRequired();

            builder.Property(e => e.Enabled)
                .HasDefaultValue(false);

            builder.Property(e => e.IsDefault)
                .HasDefaultValue(false);

            builder.HasMany<ApplicationUser>().WithOne().HasForeignKey(user => user.LanguageId).IsRequired();
        }
    }
}
