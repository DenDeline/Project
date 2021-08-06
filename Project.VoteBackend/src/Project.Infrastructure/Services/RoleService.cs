using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Project.ApplicationCore.Interfaces;
using Project.Infrastructure.Data;

namespace Project.Infrastructure.Services
{
  public class RoleService: IRoleService
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(
      UserManager<ApplicationUser> userManager,
      RoleManager<ApplicationRole> roleManager)
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }
    
    public async Task<Result<IReadOnlyList<string>>> AddRolesByUserNameAsync(
      string currentUserName, 
      string updatingUserName, 
      IReadOnlyList<string> updatingRoles, 
      CancellationToken cts = new CancellationToken())
    {
      var currentUser = await _userManager.FindByNameAsync(currentUserName);
      
      if (currentUser is null)
      {
        return Result<IReadOnlyList<string>>.NotFound();
      }
      
      var updatingUser = await _userManager.FindByNameAsync(updatingUserName);

      if (updatingUser is null)
      {
        return Result<IReadOnlyList<string>>.NotFound();
      }

      var canUpdateUserRolesResult = await CanUpdateUserRolesAsync(currentUser, updatingUser, updatingRoles);

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
      string currentUserName, 
      string updatingUserName, 
      IReadOnlyList<string> updatingRoles, 
      CancellationToken cts = new CancellationToken())
    {
      var currentUser = await _userManager.FindByNameAsync(currentUserName);
      
      if (currentUser is null)
      {
        return Result<IReadOnlyList<string>>.NotFound();
      }
      
      var updatingUser = await _userManager.FindByNameAsync(updatingUserName);

      if (updatingUser is null)
      {
        return Result<IReadOnlyList<string>>.NotFound();
      }

      var canUpdateUserRolesResult = await CanUpdateUserRolesAsync(currentUser,updatingUser, updatingRoles);

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
    
    private async Task<Result<IReadOnlyList<ApplicationRole>>> GetIdentityRolesAsync(
      IReadOnlyList<string> roleNames)
    {
      var identityUpdatingRoles = new List<ApplicationRole>();

      foreach (var roleName in roleNames)
      {
        var identityUpdatingRole = await _roleManager.FindByNameAsync(roleName);
        if (identityUpdatingRole is null)
        {
          return Result<IReadOnlyList<ApplicationRole>>.Error("Role doesn't exist");
        }
        identityUpdatingRoles.Add(identityUpdatingRole);
      }

      return identityUpdatingRoles.AsReadOnly();
    }

    private Result<ApplicationRole> GetMainRole(IReadOnlyList<ApplicationRole> roles)
    {
      if (!roles.Any())
      {
        return Result<ApplicationRole>.NotFound();
      }

      return roles.OrderBy(_ => _.Position).Last();
    }

    private async Task<Result<bool>> CanUpdateUserRolesAsync(
      ApplicationUser currentUser,
      ApplicationUser updatingUser,
      IReadOnlyList<string> updatingRoles)
    {
      // TODO: Add fluent validation for updatingRoles

      if (!updatingRoles.Any()) 
        return Result<bool>.Error();
      
      var updatingIdentityRolesResult = await GetIdentityRolesAsync(updatingRoles);
      if (!updatingIdentityRolesResult.IsSuccess)
        return Result<bool>.Error(updatingIdentityRolesResult.Errors.ToArray());


      var currentUserRoles = (await _userManager.GetRolesAsync(currentUser))
        .ToList()
        .AsReadOnly();
      
      if (!currentUserRoles.Any())
        return Result<bool>.Forbidden();

      var currentUserIdentityRolesResult = await GetIdentityRolesAsync(currentUserRoles);
      if (!currentUserIdentityRolesResult.IsSuccess)
        return Result<bool>.Error(currentUserIdentityRolesResult.Errors.ToArray());
      
      
      if (currentUserIdentityRolesResult.Value.Max(_ => _.Position) <=
          updatingIdentityRolesResult.Value.Max(_ => _.Position))
      {
        return Result<bool>.Forbidden();
      }
      
      var updatingUserRoles = (await _userManager.GetRolesAsync(updatingUser))
        .ToList()
        .AsReadOnly();

      if (updatingUserRoles.Any())
      {
        var updatingUserIdentityRolesResult = await GetIdentityRolesAsync(currentUserRoles);
        if (!updatingUserIdentityRolesResult.IsSuccess)
          return Result<bool>.Error(currentUserIdentityRolesResult.Errors.ToArray());

        if (updatingUserIdentityRolesResult.Value.Max(_ => _.Position) >=
            currentUserIdentityRolesResult.Value.Max(_ => _.Position))
        {
          return Result<bool>.Forbidden();
        }
      };

      
      return true;
    }
  }
}
