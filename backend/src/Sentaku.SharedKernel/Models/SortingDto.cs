using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Sentaku.SharedKernel.Enums;

namespace Sentaku.SharedKernel.Models;

public class SortingDto<T>
{
  public T ColumnType { get; set; }
  
  [EnumDataType(typeof(SortingOrder))]
  public SortingOrder Order { get; set; }
}
