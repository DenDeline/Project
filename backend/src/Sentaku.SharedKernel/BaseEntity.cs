using System;

namespace Sentaku.SharedKernel
{
  public class BaseEntity<TId>
  {
    public TId Id { get; set; }
  }
  
  public class BaseEntity
  {
    public Guid Id { get; set; }
  }
}
