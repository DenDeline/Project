using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;

namespace Sentaku.Infrastructure.Data.Config;

public class VoterConfig: IEntityTypeConfiguration<Voter>
{
  public void Configure(EntityTypeBuilder<Voter> builder)
  {
    builder
      .Property(_ => _.IdentityId)
      .HasConversion<Guid>();

    builder
      .Property(_ => _.CreatedOn)
      .IsRequired();

    builder
      .HasOne<AppUser>()
      .WithOne()
      .HasForeignKey<Voter>(_ => _.IdentityId)
      .IsRequired();

    builder
      .Property(_ => _.IsArchived)
      .IsRequired();
    
    builder
      .HasMany<Vote>()
      .WithOne(_ => _.Voter)
      .HasForeignKey(_ => _.VoterId);
    
    builder
      .HasMany(_ => _.JoinedSessions)
      .WithOne(_ => _.Voter)
      .HasForeignKey(_ => _.VoterId);
  }
}
