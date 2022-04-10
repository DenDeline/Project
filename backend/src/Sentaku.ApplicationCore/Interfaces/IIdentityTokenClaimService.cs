using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Sentaku.ApplicationCore.ValueObjects;

namespace Sentaku.ApplicationCore.Interfaces
{
  public interface IIdentityTokenClaimService
  {
    Task<Result<AuthTokenResult>> GetTokenAsync(
      string userId,
      string issuer,
      string audience,
      CancellationToken cancellationToken = default);
  }
}
