using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.IdentityModel.Tokens;
using Project.ApplicationCore.Entities;

namespace Project.ApplicationCore.Interfaces
{
    public interface ITokenService
    {
        Task<Result<string>> CreateAccessTokenAsync(int userId, SigningCredentials signinKey, CancellationToken ctsToken = new CancellationToken());
    }
}