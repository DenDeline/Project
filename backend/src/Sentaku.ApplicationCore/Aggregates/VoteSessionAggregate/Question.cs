using System;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Sentaku.SharedKernel;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;

public class Question: BaseEntity<Guid>
{
  public int Index { get; set; }
  [JsonIgnore]
  public VoteSession VoteSession { get; }
  public Guid VoteSessionId { get; }
  
  public string Summary { get; private set; }
  public string Description { get; private set; }
  public DateTime CreatedOn { get; }
  
  // EF CORE
  private Question() {}
  public Question(VoteSession session, int index, string summary, string description)
  {
    VoteSession = Guard.Against.Null(session);
    Summary = Guard.Against.NullOrWhiteSpace(summary, nameof(summary));
    Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
    Index = index;
    CreatedOn = DateTime.UtcNow;
  }

  public void UpdatePrimaryInfo(string summary, string description)
  {
    Summary = Guard.Against.NullOrWhiteSpace(summary, nameof(summary));
    Description = Guard.Against.NullOrWhiteSpace(description, nameof(description));
  }
}
