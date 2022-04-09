using System;
using System.Collections.Generic;

namespace Sentaku.SharedKernel.Interfaces;

public interface IIdentityEntity<TId> : IEntity<TId>, IIdentityEntity
{
  
}

public interface IIdentityEntity: IEntity
{
  string IdentityId { get; }
  DateTime CreatedOn { get; }
  bool IsArchived { get; }
  DateTime? ArchivedOn { get; }

  void RestoreIdentity();
  void ArchiveIdentity();
}


public interface IEntity<TId>: IEntity
{
  TId Id { get; }
}

public interface IEntity
{
  List<IDomainEvent> Events { get; }
}
