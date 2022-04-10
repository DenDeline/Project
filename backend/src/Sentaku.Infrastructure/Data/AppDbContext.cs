using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Aggregates;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate;
using Sentaku.ApplicationCore.Aggregates.VoteSessionAggregate.Enums;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.SharedKernel;
using Sentaku.SharedKernel.Interfaces;
using SmartEnum.EFCore;

namespace Sentaku.Infrastructure.Data
{
  public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
  {
    private readonly IMediator? _mediator;
    public AppDbContext(
      DbContextOptions<AppDbContext> options,
      IMediator? mediator = null) : base(options)
    {
      _mediator = mediator;
    }
    
    public DbSet<VoteSession> VoteSessions { get; set; }

    public DbSet<VotingManager> VotingManagers { get; set; } 
    
    public DbSet<Voter> Voters { get; set; }
    public DbSet<Language> Languages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);
      

      modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
      var result = await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

      if (_mediator is null) return result;
      
      var entitiesWithEvents = ChangeTracker.Entries<IEntity>()
        .Select(e => e.Entity)
        .Where(e => e.Events.Any())
        .ToArray();

      foreach (var entity in entitiesWithEvents)
      {
        var events = entity.Events.ToArray();
        entity.Events.Clear();
        foreach (var domainEvent in events)
        {
          await _mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
        }
      }

      return result;
    }
  }
}
