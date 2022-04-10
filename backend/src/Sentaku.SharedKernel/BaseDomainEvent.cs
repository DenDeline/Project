using System;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.SharedKernel;

public class BaseDomainEvent: IDomainEvent
{
  public DateTime DateOccured { get; private set; } = DateTime.UtcNow;
}
