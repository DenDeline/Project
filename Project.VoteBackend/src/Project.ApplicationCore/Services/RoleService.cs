using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Project.ApplicationCore.Entities;
using Project.ApplicationCore.Interfaces;

namespace Project.ApplicationCore.Services
{
  public class RoleService: IRoleService
  {
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole<int>> _roleManager;

    public RoleService(
      UserManager<AppUser> userManager,
      RoleManager<IdentityRole<int>> roleManager)
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }
    
    public async Task<Result<IReadOnlyList<string>>> AddRolesByUserNameAsync(
      AppUser currentUser, 
      string username, 
      IReadOnlyList<string> updatingRoles, 
      CancellationToken cts = new CancellationToken())
    {
      var updatingUser = await _userManager.FindByNameAsync(username);

      if (updatingUser is null)
      {
        return Result<IReadOnlyList<string>>.NotFound();
      }

      var canUpdateUserRolesResult = await CanUpdateUserRolesAsync(currentUser, updatingRoles);

      if (!canUpdateUserRolesResult.IsSuccess)
      {
        switch (canUpdateUserRolesResult.Status)
        {
          case ResultStatus.Error:
            return Result<IReadOnlyList<string>>.Error(canUpdateUserRolesResult.Errors.ToArray());
          case ResultStatus.Forbidden:
            return Result<IReadOnlyList<string>>.Forbidden();
          case ResultStatus.Invalid:
            return Result<IReadOnlyList<string>>.Invalid(canUpdateUserRolesResult.ValidationErrors);
          case ResultStatus.NotFound:
            return Result<IReadOnlyList<string>>.NotFound();
        }
      }
      
      var result = await _userManager.AddToRolesAsync(updatingUser, updatingRoles);
      if (!result.Succeeded)
      {
        return Result<IReadOnlyList<string>>.Error("");
      }
        
      var updatedUserRoles = (await _userManager.GetRolesAsync(updatingUser))
        .ToList()
        .AsReadOnly();
        
      return Result<IReadOnlyList<string>>.Success(updatedUserRoles);
    }
    
    public async Task<Result<IReadOnlyList<string>>> RemoveRolesByUserNameAsync(
      AppUser currentUser, 
      string username, 
      IReadOnlyList<string> updatingRoles, 
      CancellationToken cts = new CancellationToken())
    {
      var updatingUser = await _userManager.FindByNameAsync(username);

      if (updatingUser is null)
      {
        return Result<IReadOnlyList<string>>.NotFound();
      }

      var canUpdateUserRolesResult = await CanUpdateUserRolesAsync(currentUser, updatingRoles);

      if (!canUpdateUserRolesResult.IsSuccess)
      {
        switch (canUpdateUserRolesResult.Status)
        {
          case ResultStatus.Error:
            return Result<IReadOnlyList<string>>.Error(canUpdateUserRolesResult.Errors.ToArray());
          case ResultStatus.Forbidden:
            return Result<IReadOnlyList<string>>.Forbidden();
          case ResultStatus.Invalid:
            return Result<IReadOnlyList<string>>.Invalid(canUpdateUserRolesResult.ValidationErrors);
          case ResultStatus.NotFound:
            return Result<IReadOnlyList<string>>.NotFound();
        }
      }
      
      var result = await _userManager.RemoveFromRolesAsync(updatingUser, updatingRoles);
      if (!result.Succeeded)
      {
        return Result<IReadOnlyList<string>>.Error("");
      }
        
      var updatedUserRoles = (await _userManager.GetRolesAsync(updatingUser))
        .ToList()
        .AsReadOnly();
        
      return Result<IReadOnlyList<string>>.Success(updatedUserRoles);
    }
    
    // ===============================================================================================================================//
    // PRIVATE METHODS                                                                                                                //
    // ===============================================================================================================================//
    
    private async Task<Result<IReadOnlyList<IdentityRole<int>>>> GetIdentityRolesAsync(
      IReadOnlyList<string> roleNames)
    {
      var identityUpdatingRoles = new List<IdentityRole<int>>();

      foreach (var roleName in roleNames)
      {
        var identityUpdatingRole = await _roleManager.FindByNameAsync(roleName);
        if (identityUpdatingRole is null)
        {
          return Result<IReadOnlyList<IdentityRole<int>>>.Error("Role doesn't exist");
        }
        identityUpdatingRoles.Add(identityUpdatingRole);
      }

      return identityUpdatingRoles.AsReadOnly();
    }

    private Result<IdentityRole<int>> GetMainRole(IReadOnlyList<IdentityRole<int>> roles)
    {
      if (!roles.Any())
      {
        return Result<IdentityRole<int>>.NotFound();
      }

      return roles.OrderBy(_ => _.Id).First();
    }

    private async Task<Result<bool>> CanUpdateUserRolesAsync(
      AppUser currentUser,
      IReadOnlyList<string> updatingRoles)
    {
      // TODO: Add fluent validation for updatingRoles

      if (!updatingRoles.Any())
      {
        return Result<bool>.Error();
      }
      
      var identityUpdatingRolesResult = await GetIdentityRolesAsync(updatingRoles);
      if (!identityUpdatingRolesResult.IsSuccess)
      {
        return Result<bool>.Error(identityUpdatingRolesResult.Errors.ToArray());
      }
      
      var currentUserRoles = (await _userManager.GetRolesAsync(currentUser))
        .ToList()
        .AsReadOnly();
      
      if (!currentUserRoles.Any())
      {
        return Result<bool>.Forbidden();
      }
      
      var currentUserIdentityRolesResult = await GetIdentityRolesAsync(currentUserRoles);
      if (!identityUpdatingRolesResult.IsSuccess)
      {
        return Result<bool>.Error(identityUpdatingRolesResult.Errors.ToArray());
      }

      var currentUserMainRoleResult = GetMainRole(currentUserIdentityRolesResult.Value);
      if (!currentUserMainRoleResult.IsSuccess)
      {
        return Result<bool>.Forbidden();
      }
      
      if (identityUpdatingRolesResult.Value.Any(r => r.Id < currentUserMainRoleResult.Value.Id))
      {
        return Result<bool>.Forbidden();
      }

      return true;
    }
  }
}
