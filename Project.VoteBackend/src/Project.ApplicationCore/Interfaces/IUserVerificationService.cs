using System.Threading.Tasks;
using Ardalis.Result;
using Project.ApplicationCore.Aggregates;

namespace Project.ApplicationCore.Interfaces
{
  public interface IUserVerificationService
  {
    Task<Result<bool>> UpdateProfileImageByUsernameAsync(string username, string untrustedFileName, byte[] content, string contentType); 
    Task<Result<AppFile>> GetProfileImageByUsernameAsync(string username);
    Task<Result<bool>> UpdateUserVerificationByUsernameAsync(string currentUsername, string username, bool verified);
  }
}
