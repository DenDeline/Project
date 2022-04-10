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
using Sentaku.ApplicationCore.Aggregates.VoterAggregate.Specifications;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate;
using Sentaku.ApplicationCore.Aggregates.VotingManagerAggregate.Specifications;
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel;
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
    
    public async Task<Result> ArchiveVotingManagerByIdAsync(
      Guid managerId,
      CancellationToken cancellationToken = default)
    {
      var votingManager = await _votingManagerRepository.GetByIdAsync(managerId, cancellationToken);
      
      if (votingManager is null)
        return Result.NotFound();

      var user = await _userManager.FindByIdAsync(votingManager.IdentityId);
      
      if (user is null)
        return Result.NotFound();
      
      votingManager.ArchiveIdentity();
      await _votingManagerRepository.UpdateAsync(votingManager, cancellationToken);
      
      await _userManager.RemoveFromRoleAsync(user, RolesEnum.LeadManager.Name);

      return Result.Success();
    }
    
    public async Task<Result> RestoreVotingManagerByIdAsync(
      Guid managerId,
      CancellationToken cancellationToken = default)
    {
      var votingManager = await _votingManagerRepository.GetByIdAsync(managerId, cancellationToken);
      
      if (votingManager is null)
        return Result.NotFound();

      var user = await _userManager.FindByIdAsync(votingManager.IdentityId);

      votingManager.RestoreIdentity();
      await _votingManagerRepository.UpdateAsync(votingManager, cancellationToken);
      
      await _userManager.AddToRoleAsync(user, RolesEnum.LeadManager.Name);

      return Result.Success();
    }

    public async Task<Result> DeleteVotingManagerByIdAsync(
      Guid managerId,
      CancellationToken cancellationToken = default)
    {
      var votingManager = await _votingManagerRepository.GetByIdAsync(managerId, cancellationToken);
      
      if (votingManager is null)
        return Result.NotFound();

      var user = await _userManager.FindByIdAsync(votingManager.IdentityId);

      await _votingManagerRepository.DeleteAsync(votingManager, cancellationToken);
      await _userManager.RemoveFromRoleAsync(user, RolesEnum.LeadManager.Name);
      
      return Result.Success();
    }

    public async Task<Result<Voter>> CreateVoterByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
      Guard.Against.NullOrWhiteSpace(username, nameof(username));

      var user = await _userManager.FindByNameAsync(username);
      
      if (user is null)
        return Result<Voter>.NotFound();

      var spec = new VoterByIdentitySpec(user.Id);

      var votingManagerExists = await _voterRepository.AnyAsync(spec, cancellationToken);

      if (votingManagerExists)
        return Result<Voter>.Error("User is already voter");

      await _userManager.AddToRoleAsync(user, RolesEnum.Authority.Name);

      var voter = new Voter(user.Id);
      
      return await _voterRepository.AddAsync(voter, cancellationToken);
    }

    public async Task<Result> ArchiveVoterByIdAsync(Guid voterId, CancellationToken cancellationToken = default)
    {
      var voter = await _voterRepository.GetByIdAsync(voterId, cancellationToken);
      
      if (voter is null)
        return Result.NotFound();

      var user = await _userManager.FindByIdAsync(voter.IdentityId);
      
      if (user is null)
        return Result.NotFound();
      
      
      voter.ArchiveIdentity();
      await _voterRepository.UpdateAsync(voter, cancellationToken);
      
      await _userManager.RemoveFromRoleAsync(user, RolesEnum.Authority.Name);

      return Result.Success();
    }

    public async Task<Result> RestoreVoterByIdAsync(Guid voterId, CancellationToken cancellationToken = default)
    {
      var voter = await _voterRepository.GetByIdAsync(voterId, cancellationToken);
      
      if (voter is null)
        return Result.NotFound();

      var user = await _userManager.FindByIdAsync(voter.IdentityId);
      
      if (user is null)
        return Result.NotFound();
      
      voter.RestoreIdentity();
      await _voterRepository.UpdateAsync(voter, cancellationToken);
      
      await _userManager.AddToRoleAsync(user, RolesEnum.Authority.Name);

      return Result.Success();

    }

    public async Task<Result> DeleteVoterByIdAsync(Guid voterId, CancellationToken cancellationToken = default)
    {
      var voter = await _voterRepository.GetByIdAsync(voterId, cancellationToken);
      
      if (voter is null)
        return Result.NotFound();

      var user = await _userManager.FindByIdAsync(voter.IdentityId);

      await _voterRepository.DeleteAsync(voter, cancellationToken);
      await _userManager.RemoveFromRoleAsync(user, RolesEnum.Authority.Name);
      
      return Result.Success();
    }
  }
}
