using System.Threading.Tasks;
using Ardalis.Result;
using Sentaku.SharedKernel.Constants;

namespace Sentaku.ApplicationCore.Interfaces
{
  public interface IPermissionsService
  {
    public Task<Result<Permissions>> GetPermissionsByUsernameAsync(string username);
  }
}
