using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Services
{
  public class RoleService: IRoleService
  {
    private readonly AppDbContext _context;

    public RoleService(
      AppDbContext context)
    {
      _context = context;
    }

    public async Task<Result<IReadOnlyList<string>>> UpdateRolesByUsernameAsync(
      string currentUsername,  
      string updatingUsername, 
      IReadOnlyList<string> updatingRoles,
      CancellationToken cancellationToken = default)
    {
      var currentUser = await _context.Users
        .Where(user => user.NormalizedUserName == currentUsername.ToUpperInvariant())
        .Select(user => new { user.Id })
        .FirstOrDefaultAsync(cancellationToken);
      
      if (currentUser is null)
        return Result<IReadOnlyList<string>>.Forbidden();

      var updatingUser = await _context.Users
        .Where(user => user.NormalizedUserName == updatingUsername.ToUpperInvariant())
        .Select(user => new { user.Id, user.Verified })
        .FirstOrDefaultAsync(cancellationToken);

      if (updatingUser is null)
        return Result<IReadOnlyList<string>>.NotFound();
      
      if (!updatingUser.Verified)
        return Result<IReadOnlyList<string>>.Forbidden();
      
      var normalizedUpdatingRoles = updatingRoles
        .Select(updatingRole => updatingRole.ToUpperInvariant())
        .ToList();
      
      var canUpdateUserRolesResult = await CanUpdateUserRolesAsync(currentUser.Id, updatingUser.Id, updatingRoles, cancellationToken);
      
      //TODO: handle other response types 
      if (!canUpdateUserRolesResult.IsSuccess)
        return Result<IReadOnlyList<string>>.Forbidden();

      var updatingUserRoles = await _context.Roles
        .Where(role => normalizedUpdatingRoles.Contains(role.NormalizedName))
        .Select(role => new IdentityUserRole<string> { UserId = updatingUser.Id, RoleId = role.Id})
        .ToListAsync(cancellationToken);
      
      var oldUserRoles = await _context.UserRoles
        .Where(_ => _.UserId == updatingUser.Id)
        .ToListAsync(cancellationToken);

      _context.UserRoles.RemoveRange(oldUserRoles.Except(updatingUserRoles));
      _context.UserRoles.AddRange(updatingUserRoles.Except(oldUserRoles));
      
      await _context.SaveChangesAsync(cancellationToken);
      return Result<IReadOnlyList<string>>.Success(updatingRoles);
    }

    public async Task<Result<IReadOnlyList<string>>> DeleteUserRolesByUsernameAsync(
      string currentUsername, 
      string updatingUsername,
      CancellationToken cancellationToken = default)
    {
      var currentUser = await _context.Users
        .Where(user => user.NormalizedUserName == currentUsername.ToUpperInvariant())
        .Select(user => new { user.Id })
        .FirstOrDefaultAsync(cancellationToken);
      
      if (currentUser is null)
        return Result<IReadOnlyList<string>>.Forbidden();

      var updatingUser = await _context.Users
        .Where(user => user.NormalizedUserName == updatingUsername.ToUpperInvariant())
        .Select(user => new { user.Id, user.Verified })
        .FirstOrDefaultAsync(cancellationToken);

      if (updatingUser is null)
        return Result<IReadOnlyList<string>>.NotFound();
      
      if (!updatingUser.Verified)
        return Result<IReadOnlyList<string>>.Forbidden();

      var updatingRoles = new List<string>().AsReadOnly();
      
      var canUpdateUserRolesResult = await CanUpdateUserRolesAsync(
        currentUser.Id, 
        updatingUser.Id,
        //TODO: remove param
        updatingRoles,
        cancellationToken);
      
      //TODO: handle other response types 
      if (!canUpdateUserRolesResult.IsSuccess)
        return Result<IReadOnlyList<string>>.Forbidden();
      
      var oldUserRoles = await _context.UserRoles
        .AsNoTracking()
        .Where(_ => _.UserId == updatingUser.Id)
        .ToListAsync(cancellationToken);
      
      _context.UserRoles.RemoveRange(oldUserRoles);
      await _context.SaveChangesAsync(cancellationToken);
      
      return Result<IReadOnlyList<string>>.Success(updatingRoles);
    }

    private async Task<Result<bool>> CanUpdateUserRolesAsync(
      string currentUserId,
      string updatingUserId,
      //TODO: add nullable support
      IReadOnlyList<string> updatingRoles,
      CancellationToken cancellationToken = default)
    {
      var roles =  _context.Roles;

      var roleNames = await roles
        .Select(_ => _.Name)
        .ToListAsync(cancellationToken);

      if (!updatingRoles.All(_ => roleNames.Contains(_)))
        return Result<bool>.Error();

      var currentUserRoleMaxPosition = await _context.UserRoles
        .Where(_ => _.UserId == currentUserId)
        .Join(roles, nav => nav.RoleId, role => role.Id, (nav, role) => new { role.Position })
        .MaxAsync(role => (int?) role.Position, cancellationToken) ?? 0;
      
      var updatingUserRoleMaxPosition =  await _context.UserRoles
        .Where(_ => _.UserId == updatingUserId)
        .Join(roles, nav => nav.RoleId, role => role.Id, (nav, role) => new { role.Position })
        .MaxAsync(role => (int?) role.Position, cancellationToken) ?? 0;

      var updatingRolesMaxPosition = await _context.Roles
        .Where(role => updatingRoles.Contains(role.Name))
        .MaxAsync(role => (int?)role.Position, cancellationToken) ?? 0;

      return (
        currentUserRoleMaxPosition > updatingUserRoleMaxPosition && 
        currentUserRoleMaxPosition > updatingRolesMaxPosition
        ) ? Result<bool>.Success(true) : Result<bool>.Forbidden();
    }
  }
}
