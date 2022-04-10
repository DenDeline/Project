using Ardalis.SmartEnum;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

public class VoteTypes: SmartEnum<VoteTypes>
{
  public static readonly VoteTypes For = new(nameof(For), 1);
  public static readonly VoteTypes Against = new(nameof(Against), 2);
  public static readonly VoteTypes Abstain = new(nameof(Abstain), 3);
  
  public VoteTypes(string name, int value) : base(name, value)
  {
  }
}
