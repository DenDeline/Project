using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.ValueObjects;

namespace Sentaku.Infrastructure.Data.Config;

public class JoinedVotersSessionsConfig: IEntityTypeConfiguration<JoinedVotersSessions>
{
  public void Configure(EntityTypeBuilder<JoinedVotersSessions> builder)
  {
    builder.HasKey(_ => new { _.VoterId, _.VoteSessionId });

    builder
      .Property(_ => _.IsFavorite)
      .IsRequired();
    
    builder
      .Property(_ => _.IsVoted)
      .IsRequired();
  }
}
