using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Ardalis.GuardClauses;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
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

  private readonly List<Vote> _votes = new();
  public IReadOnlyList<Vote> Votes => _votes.AsReadOnly();
  
  private readonly List<VotingResult> _results = new();
  public IReadOnlyList<VotingResult> Results => _results.AsReadOnly();

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

  public void AddVote(Voter voter, VoteTypes type) => _votes.Add(new Vote(this, voter, type));

  public void CalculateResults()
  {
    var results= _votes.GroupBy(v => v.Type, v => v.VoterId, (type, voters) => new VotingResult(this, type, voters.Count()));
    _results.AddRange(results);
  }
}
