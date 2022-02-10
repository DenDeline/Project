using Project.SharedKernel;
using Project.SharedKernel.Interfaces;

namespace Project.ApplicationCore.Aggregates
{
  public class Language : BaseEntity<int>, IAggregateRoot
  {
    public string Name { get; set; }
    public string Code { get; set; }
    public bool Enabled { get; set; }
    public bool IsDefault { get; set; }
  }
}
