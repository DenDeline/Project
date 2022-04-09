using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sentaku.ApplicationCore.Aggregates.VoterAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel.Constants;
using Sentaku.SharedKernel.Interfaces;

namespace Sentaku.Infrastructure.Services
{
  public class RoleService : IRoleService
  {
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IRepository<VotingManager> _votingManagerRepository;
    private readonly IRepository<Voter> _voterRepository;
    private readonly RoleManager<AppRole> _roleManager;

    public RoleService(
      AppDbContext context,
      UserManager<AppUser> userManager,
      IRepository<VotingManager> votingManagerRepository,
      IRepository<Voter> voterRepository,
      RoleManager<AppRole> roleManager)
    {
      _context = context;
      _userManager = userManager;
      _votingManagerRepository = votingManagerRepository;
      _voterRepository = voterRepository;
      _roleManager = roleManager;
    }

    public async Task<Result<VotingManager>> CreateVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default)
    {
      Guard.Against.NullOrWhiteSpace(username, nameof(username));

      var user = await _userManager.FindByNameAsync(username);
      
      if (user is null)
        return Result<VotingManager>.NotFound();

      var spec = new VotingManagerByIdentitySpec(user.Id);

      var votingManagerExists = await _votingManagerRepository.AnyAsync(spec, cancellationToken);

      if (votingManagerExists)
        return Result<VotingManager>.Error("User is already vote manager");

      await _userManager.AddToRoleAsync(user, RolesEnum.LeadManager.Name);

      var votingManager = new VotingManager(user.Id);
      
      return await _votingManagerRepository.AddAsync(votingManager, cancellationToken);
    }
    
    public async Task<Result> ArchiveVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default)
    {
      var user = await _userManager.FindByNameAsync(username);
      
      if (user is null)
        return Result.NotFound();

      var spec = new VotingManagerByIdentitySpec(user.Id);
      
      var votingManager = await _votingManagerRepository.GetBySpecAsync(spec, cancellationToken);
      
      if (votingManager is null)
        return Result.NotFound();

      votingManager.ArchiveIdentity();
      await _votingManagerRepository.UpdateAsync(votingManager, cancellationToken);
      
      await _userManager.RemoveFromRoleAsync(user, RolesEnum.LeadManager.Name);

      return Result.Success();
    }
    
    public async Task<Result> RestoreVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default)
    {
      var user = await _userManager.FindByNameAsync(username);
      
      if (user is null)
        return Result.NotFound();

      var spec = new VotingManagerByIdentitySpec(user.Id);
      
      var votingManager = await _votingManagerRepository.GetBySpecAsync(spec, cancellationToken);
      
      if (votingManager is null)
        return Result.NotFound();

      votingManager.RestoreIdentity();
      await _votingManagerRepository.UpdateAsync(votingManager, cancellationToken);
      
      await _userManager.AddToRoleAsync(user, RolesEnum.LeadManager.Name);

      return Result.Success();
    }

    public async Task<Result> DeleteVotingManagerByUsernameAsync(
      string username,
      CancellationToken cancellationToken = default)
    {
      var user = await _userManager.FindByNameAsync(username);
      
      if (user is null)
        return Result.NotFound();

      var spec = new VotingManagerByIdentitySpec(user.Id);
      
      var votingManager = await _votingManagerRepository.GetBySpecAsync(spec, cancellationToken);
      
      if (votingManager is null)
        return Result.NotFound();

      await _votingManagerRepository.DeleteAsync(votingManager, cancellationToken);
      await _userManager.RemoveFromRoleAsync(user, RolesEnum.LeadManager.Name);
      
      return Result.Success();
    }

    public async Task<Result<bool>> AssignAuthorityByUsername(
      ClaimsPrincipal currentUser,
      string username,
      CancellationToken cancellationToken = default)
    {
      var currentUserId = _userManager.GetUserId(currentUser);
      
      if (currentUserId is null)
        return Result<bool>.Forbidden();

      return true;
    }

    public async Task<Result<bool>> AssignRepresentativeAuthorityByUsername(
      ClaimsPrincipal currentUser,
      string username,
      CancellationToken cancellationToken = default)
    {
      var currentUserId = _userManager.GetUserId(currentUser);
      
      if (currentUserId is null)
        return Result<bool>.Forbidden();

      var updatingUserResponse = await _userManager.Users
        .Where(_ => _.NormalizedUserName == _userManager.NormalizeName(username))
        .Select(_ => _.Id)
        .FirstOrDefaultAsync(cancellationToken);
      
      if (updatingUserResponse is null)
        return Result<bool>.NotFound();
      
      
      return true;
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
        .Select(role => new IdentityUserRole<string> { UserId = updatingUser.Id, RoleId = role.Id })
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
      var roles = _context.Roles;

      var roleNames = await roles
        .Select(_ => _.Name)
        .ToListAsync(cancellationToken);

      if (!updatingRoles.All(_ => roleNames.Contains(_)))
        return Result<bool>.Error();

      var currentUserRoleMaxPosition = await _context.UserRoles
        .Where(_ => _.UserId == currentUserId)
        .Join(roles, nav => nav.RoleId, role => role.Id, (nav, role) => new { role.Position })
        .MaxAsync(role => (int?)role.Position, cancellationToken) ?? 0;

      var updatingUserRoleMaxPosition = await _context.UserRoles
        .Where(_ => _.UserId == updatingUserId)
        .Join(roles, nav => nav.RoleId, role => role.Id, (nav, role) => new { role.Position })
        .MaxAsync(role => (int?)role.Position, cancellationToken) ?? 0;

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
