using System;
using System.Collections.Generic;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.SharedKernel
{
  public class BaseEntity<TId>: IEntity<TId>
  {
    private List<IDomainEvent> _events = new();
    
    public TId Id { get; set; }
    public List<IDomainEvent> Events => _events;
  }
}
