using System.Threading.Tasks;

namespace Sentaku.ApplicationCore.Interfaces
{
  public interface IIdentityTokenClaimService
  {
    Task<string> GetTokenAsync(string username, string issuer, string audience);
  }
}
