using Ardalis.SmartEnum;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

public class SessionState: SmartEnum<SessionState>
{
  public static readonly SessionState Pending = new(nameof(Pending), 1);
  public static readonly SessionState Active = new(nameof(Active), 2);
  public static readonly SessionState Closed = new(nameof(Closed), 3);
  public static readonly SessionState ResultsApproved = new(nameof(ResultsApproved), 4);
  
  public SessionState(string name, int value) : base(name, value)
  {
  }
}
