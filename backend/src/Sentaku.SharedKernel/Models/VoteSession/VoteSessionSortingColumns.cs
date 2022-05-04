using System.Text.Json.Serialization;
using Ardalis.SmartEnum;

namespace Sentaku.SharedKernel.Models.VoteSession;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum VoteSessionSortingColumns
{
  CreatedOn,
  ResultsApprovedOn
}
