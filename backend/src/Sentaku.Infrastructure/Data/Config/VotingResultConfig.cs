using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

namespace Sentaku.Infrastructure.Data.Config;

public class VotingResultConfig: IEntityTypeConfiguration<VotingResult>
{
  public void Configure(EntityTypeBuilder<VotingResult> builder)
  {
    builder.HasKey(_ => new { _.QuestionId, _.Type });

    builder
      .Property(_ => _.Type)
      .HasConversion(p => p.Value, p => VoteTypes.FromValue(p));

    builder
      .Property(_ => _.Count)
      .IsRequired();
  }
}
