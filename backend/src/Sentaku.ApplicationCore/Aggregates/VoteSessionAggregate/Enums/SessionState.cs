using System.ComponentModel.DataAnnotations.Schema;
using Ardalis.SmartEnum;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;

public abstract class SessionState: SmartEnum<SessionState, int>
{
  public static readonly SessionState Pending = new PendingState();
  public static readonly SessionState Active = new ActiveState();
  public static readonly SessionState Closed = new ClosedState();
  public static readonly SessionState ResultsApproved = new ResultsApprovedState();
  
  public abstract SessionState? NextState { get; }
  
  public abstract bool CanEditSession { get; }

  private SessionState(string name, int value) : base(name, value)
  {
  }

  private sealed class PendingState: SessionState
  {
    public PendingState() : base(nameof(Pending), 1)
    {
    }

    public override SessionState? NextState => Active;
    public override bool CanEditSession => true;
  }
  
  private sealed class ActiveState: SessionState
  {
    public ActiveState() : base(nameof(Active), 2)
    {
    }

    public override SessionState? NextState => Closed;
    public override bool CanEditSession => false;
  }
  
  private sealed class ClosedState: SessionState
  {
    public ClosedState() : base(nameof(Closed), 3)
    {
    }

    public override SessionState? NextState => ResultsApproved;
    public override bool CanEditSession => false;
  }
  
  private sealed class ResultsApprovedState: SessionState
  {
    public ResultsApprovedState() : base(nameof(ResultsApproved), 4)
    {
    }

    public override SessionState? NextState => null;
    public override bool CanEditSession => false;
  }
}
