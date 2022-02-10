using Ardalis.Specification.EntityFrameworkCore;
using Project.SharedKernel.Interfaces;

namespace Project.Infrastructure.Data
{
  public class EfRepository<T> : RepositoryBase<T>, IRepository<T> where T : class, IAggregateRoot
  {
    public EfRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
  }
}
