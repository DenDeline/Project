using System;
using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VoterAggregate.Specifications;

public class VoterByIdentitySpec: Specification<Voter>, ISingleResultSpecification
{
  public VoterByIdentitySpec(string identityId)
  {
    Query
      .Where(_ => _.IdentityId == identityId);
  }
}
