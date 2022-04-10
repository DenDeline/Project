using System;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoterAggregate
{
  public class Voter : IdentityEntity<Guid>, IAggregateRoot
  {
    private Voter() {}
    
    public Voter(string identityId): base(identityId)
    {
      
    }
  }
}
