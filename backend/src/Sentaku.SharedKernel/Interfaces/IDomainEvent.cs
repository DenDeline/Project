using System;
using MediatR;

namespace Sentaku.SharedKernel.Interfaces;

public interface IDomainEvent: INotification
{
  DateTime DateOccured { get; }
}
