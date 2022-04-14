using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sentaku.SharedKernel.Enums;

namespace Sentaku.SharedKernel.Models;

public class SortingDto<T>
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public T ColumnType { get; set; }
  
  [EnumDataType(typeof(SortingOrder))]
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public SortingOrder Order { get; set; }
}
