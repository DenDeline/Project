using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;

namespace Sentaku.Infrastructure.Data.Config;

public class VotingManagerConfig: IEntityTypeConfiguration<VotingManager>
{
  public void Configure(EntityTypeBuilder<VotingManager> builder)
  {
    builder
      .Property(_ => _.IdentityId)
      .HasConversion<Guid>();

    builder
      .HasMany<VoteSession>()
      .WithOne(_ => _.VotingManager)
      .HasForeignKey(_ => _.VotingManagerId);
  }
}
