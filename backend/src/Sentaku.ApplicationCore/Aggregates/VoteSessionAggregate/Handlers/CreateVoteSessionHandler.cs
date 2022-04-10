using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Events;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Handlers;

public class CreateVoteSessionHandler: INotificationHandler<CreateVoteSessionEvent>
{
  private readonly IRepository<VoteSession> _voteSessionRepository;

  public CreateVoteSessionHandler(IRepository<VoteSession> voteSessionRepository)
  {
    _voteSessionRepository = voteSessionRepository;
  }
  
  public async Task Handle(CreateVoteSessionEvent notification, CancellationToken cancellationToken)
  {
    var session = new VoteSession(notification.VotingManager, notification.Agenda, notification.StartDate)
    {
      Id = notification.VoteSessionId
    };

    await _voteSessionRepository.AddAsync(session, cancellationToken);
  }
}
