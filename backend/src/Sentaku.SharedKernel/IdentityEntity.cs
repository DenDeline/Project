using System;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.SharedKernel;

public class IdentityEntity<TId>: BaseEntity<TId>, IIdentityEntity<TId>
{
  protected IdentityEntity() {}
  
  public IdentityEntity(string identityId)
  {
    IdentityId = identityId;
    CreatedOn = DateTime.UtcNow;
  }
  
  public string IdentityId { get; }
  public DateTime CreatedOn { get; }
  
  public bool IsArchived { get; protected set; }
  public DateTime? ArchivedOn { get; protected set; }

  public virtual void RestoreIdentity()
  {
    IsArchived = false;
    ArchivedOn = null;
  }

  public virtual void ArchiveIdentity()
  {
    IsArchived = true;
    ArchivedOn = DateTime.UtcNow;
  }
}
