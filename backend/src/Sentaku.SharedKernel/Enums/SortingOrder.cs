using System.Text.Json.Serialization;
using Ardalis.SmartEnum;

namespace Sentaku.SharedKernel.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SortingOrder
{
  Ascending,
  Descending
}
