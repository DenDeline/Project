using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

namespace Sentaku.Infrastructure.Data.Config;

public class VoteConfig: IEntityTypeConfiguration<Vote>
{
  public void Configure(EntityTypeBuilder<Vote> builder)
  {
    builder.HasKey(_ => new { _.QuestionId, _.VoterId });

    builder
      .Property(_ => _.Type)
      .HasConversion(p => p.Value, p => VoteTypes.FromValue(p))
      .IsRequired();
  }
}
