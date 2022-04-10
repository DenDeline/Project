using System;
using System.Collections.Generic;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.SharedKernel;

public class BaseEntity<TId>: IEntity<TId>
{
  public TId Id { get; set; }
  public List<IDomainEvent> Events { get; } = new();
}

public class BaseEntity: IEntity
{
  public List<IDomainEvent> Events { get; } = new();
}
