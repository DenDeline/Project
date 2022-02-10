using Ardalis.Specification;

namespace Sentaku.SharedKernel.Interfaces
{
  public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
  {

  }
}
