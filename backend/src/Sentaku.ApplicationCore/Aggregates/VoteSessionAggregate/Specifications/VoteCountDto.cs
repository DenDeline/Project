using System.Text.Json.Serialization;
using Ardalis.SmartEnum.SystemTextJson;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;

public class VoteCountDto
{
  [JsonConverter(typeof(SmartEnumNameConverter<VoteTypes, int>))]
  public VoteTypes Type { get; set; }
  public int Count { get; set; }
}
