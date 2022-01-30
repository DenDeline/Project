using System.Linq;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Aggregates;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Services
{
  public class UserVerificationService: IUserVerificationService
  {
    private readonly AppDbContext _context;

    public UserVerificationService(
      AppDbContext context)
    {
      _context = context;
    }

    public async Task<Result<AppFile>> GetProfileImageByUsernameAsync(string username)
    {
      var profileImage = await _context.Users
        .Include(_ => _.ProfileImage)
        .Where(_ => _.UserName == username)
        .Select(_ => _.ProfileImage)
        .FirstOrDefaultAsync();

      if (profileImage is null)
      {
        return Result<AppFile>.NotFound();
      }

      return profileImage;
    }
    
    public async Task<Result<bool>> UpdateProfileImageByUsernameAsync(
      string username, 
      string untrustedFileName, 
      byte[] content, 
      string contentType)
    {
      var user = await _context.Users
        .Where(_ => _.UserName == username)
        .FirstOrDefaultAsync();

      if (user is null)
      {
        return Result<bool>.NotFound();
      }
      
      var image = new AppFile(untrustedFileName, content, contentType);
      user.ProfileImage = image;

      _context.Users.Update(user);
      await _context.SaveChangesAsync();
      
      return true;
    }

    public async Task<Result<bool>> UpdateUserVerificationByUsernameAsync(
      string currentUsername, 
      string username,
      bool verified)
    {
      var updatingUser = await _context.Users
        .Where(_ => _.UserName == username)
        .FirstOrDefaultAsync();

      if (updatingUser is null) 
        return Result<bool>.NotFound();

      var currentUser = await _context.Users
        .Where(_ => _.UserName == currentUsername)
        .FirstOrDefaultAsync();
      
      if (currentUser is null)
        return Result<bool>.NotFound();

      var canUpdateUserVerification = await CanUpdateUserVerificationAsync(currentUser, updatingUser);

      if (!canUpdateUserVerification)
      {
        return Result<bool>.Forbidden();
      }
      
      if (updatingUser.Verified == verified)
        return true;

      updatingUser.UpdateVerification(verified);

      _context.Users.Update(updatingUser);
      await _context.SaveChangesAsync();

      return true;
    }

    private async Task<bool> CanUpdateUserVerificationAsync(
      AppUser currentUser, 
      AppUser updatingUser)
    {
      var roles = _context.Roles;

      var currentUserRolePositions =  await _context.UserRoles
        .Where(_ => _.UserId == currentUser.Id)
        .Join(roles, nav => nav.RoleId, role => role.Id, (nav, role) => role.Position)
        .ToListAsync();
      
      var updatingUserRolePositions =  await _context.UserRoles
        .Where(_ => _.UserId == updatingUser.Id)
        .Join(roles, nav => nav.RoleId, role => role.Id, (nav, role) => role.Position)
        .ToListAsync();
      
      return (currentUserRolePositions.Any() && !updatingUserRolePositions.Any()) || 
             currentUserRolePositions.Max() > updatingUserRolePositions.Max();
    }
  }
}
