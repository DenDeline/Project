using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;

namespace Sentaku.Infrastructure.Data.Config;

public class QuestionConfig: IEntityTypeConfiguration<Question>
{
  public void Configure(EntityTypeBuilder<Question> builder)
  {
    builder
      .Property(_ => _.Summary)
      .HasMaxLength(128)
      .IsRequired()
      .IsUnicode();
    
    builder
      .Property(_ => _.Description)
      .HasMaxLength(2000)
      .IsRequired()
      .IsUnicode();

    builder
      .HasOne(_ => _.VoteSession)
      .WithMany(_ => _.Questions)
      .HasForeignKey(_ => _.VoteSessionId);

    builder
      .Property(_ => _.CreatedOn)
      .IsRequired();

    builder
      .Property(_ => _.Index)
      .IsRequired();

    builder
      .HasIndex(_ => new { _.VoteSessionId, _.Index })
      .IsUnique();
  }
}
