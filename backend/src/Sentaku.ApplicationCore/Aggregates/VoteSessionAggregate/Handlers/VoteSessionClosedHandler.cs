using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Events;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Specifications;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Handlers;

public class VoteSessionClosedHandler: INotificationHandler<VoteSessionClosedEvent>
{
  private readonly IRepository<VoteSession> _voteSessionRepository;

  public VoteSessionClosedHandler(IRepository<VoteSession> voteSessionRepository)
  {
    _voteSessionRepository = voteSessionRepository;
  }
  
  public async Task Handle(VoteSessionClosedEvent notification, CancellationToken cancellationToken)
  {
    var spec = new VoteSessionQuestionsIncludeVotesBySessionId(notification.SessionId);

    var questions = await _voteSessionRepository.GetBySpecAsync(spec, cancellationToken);

    if (questions is null)
      return;
    
    foreach (var question in questions)
      question.CalculateResults();

    await _voteSessionRepository.SaveChangesAsync(cancellationToken);
  }
}
