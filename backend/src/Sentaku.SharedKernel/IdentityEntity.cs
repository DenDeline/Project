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
  
  public bool IsDisabled { get; protected set; }
  public DateTime? DisabledOn { get; protected set; }

  public virtual void EnableIdentity()
  {
    IsDisabled = false;
    DisabledOn = null;
  }

  public virtual void DisableIdentity()
  {
    IsDisabled = true;
    DisabledOn = DateTime.UtcNow;
  }
}
