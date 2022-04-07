using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;

namespace Sentaku.Infrastructure.Data.Config;

public class VoteSessionConfig: IEntityTypeConfiguration<VoteSession>
{
  public void Configure(EntityTypeBuilder<VoteSession> builder)
  {
    
  }
}
