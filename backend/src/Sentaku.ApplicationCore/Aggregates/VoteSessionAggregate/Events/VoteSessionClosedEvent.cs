using System;
using Sentaku.SharedKernel;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Events;

public class VoteSessionClosedEvent: BaseDomainEvent
{
  public Guid SessionId { get; }

  public VoteSessionClosedEvent(Guid sessionId)
  {
    SessionId = sessionId;
  }
}
