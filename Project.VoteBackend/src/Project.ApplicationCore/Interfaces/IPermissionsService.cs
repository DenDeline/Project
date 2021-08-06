using System.Threading.Tasks;
using Ardalis.Result;
using Project.SharedKernel.Constants;

namespace Project.ApplicationCore.Interfaces
{
  public interface IPermissionsService
  {
    public Task<Result<Permissions>> GetPermissionsByUsernameAsync(string username);
  }
}
