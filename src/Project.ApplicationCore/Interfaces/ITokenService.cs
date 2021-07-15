using System.Threading.Tasks;
using Ardalis.Result;
using Project.ApplicationCore.Entities;

namespace Project.ApplicationCore.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(AppUser user, string signinKey);
    }
}