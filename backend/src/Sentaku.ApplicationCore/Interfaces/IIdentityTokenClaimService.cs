using System.Threading.Tasks;

namespace Project.ApplicationCore.Interfaces
{
  public interface IIdentityTokenClaimService
  {
    Task<string> GetTokenAsync(string username);
  }
}
