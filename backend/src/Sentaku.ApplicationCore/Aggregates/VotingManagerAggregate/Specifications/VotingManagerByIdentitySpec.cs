using Ardalis.Specification;

namespace Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;

public class VotingManagerByIdentitySpec:  Specification<VotingManager>, ISingleResultSpecification
{
  public VotingManagerByIdentitySpec(string identityId)
  {
    Query
      .Where(_ => _.IdentityId == identityId);
  }
}
