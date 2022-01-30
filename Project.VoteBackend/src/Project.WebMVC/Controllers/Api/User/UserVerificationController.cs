using System.IO;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.ApplicationCore.Interfaces;
using Project.SharedKernel.Constants;
using Project.WebMVC.Authorization.PermissionsAuthorization;
using Project.WebMVC.Models.Api.User;

namespace Project.WebMVC.Controllers.Api.User
{
  [ApiController]
  public class UserVerificationController: ControllerBase
  {
    private readonly IUserVerificationService _userVerificationService;

    public UserVerificationController(IUserVerificationService userVerificationService)
    {
      _userVerificationService = userVerificationService;
    }
    
    [Authorize]
    [HttpGet("/api/user/profileImage")]
    public async Task<ActionResult<string>> GetCurrentUserProfileImage()
    {
      if (User.Identity?.Name is null)
      {
        return Forbid();
      }
      
      var profileImageResult = await _userVerificationService.GetProfileImageByUsernameAsync(User.Identity.Name);
      
      if (!profileImageResult.IsSuccess)
      {
        return NotFound();
      }

      return File(profileImageResult.Value.Content, profileImageResult.Value.ContentType);
    }
    
    
    [Authorize]
    [HttpGet("/api/users/{username}/profileImage")]
    public async Task<ActionResult<string>> GetUserProfileImageByUsername(string username)
    {
      var profileImageResult = await _userVerificationService.GetProfileImageByUsernameAsync(username);
      
      if (!profileImageResult.IsSuccess)
      {
        return NotFound();
      }

      return File(profileImageResult.Value.Content, profileImageResult.Value.ContentType);
    }
    
    [Authorize]
    [HttpPost("/api/user/profileImage")]
    public async Task<ActionResult> UpdateUserProfileImage([FromForm] UpdateProfileImageRequest request)
    {
      if (User.Identity?.Name is null)
      {
        return Forbid();
      }
      await using MemoryStream memoryStream = new MemoryStream();
      await request.FormFile.CopyToAsync(memoryStream);
      
      if (memoryStream.Length >= 2097152)
      {
        return BadRequest();
      }
      
      var result = await _userVerificationService.UpdateProfileImageByUsernameAsync(
        User.Identity.Name, 
        request.FormFile.FileName,
        memoryStream.ToArray(),
        request.FormFile.ContentType);
        
      if (result.Status == ResultStatus.NotFound)
      {
        return NotFound();
      }

      if (!result.IsSuccess)
      {
        return Forbid();
      }
      return Ok();
    }

    [RequirePermissions(Permissions.VerifyUsers)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [HttpPost("/api/users/{username}/verify")]
    public async Task<ActionResult> UpdateUserVerification(
      [FromRoute] string username, 
      [FromBody] UpdateUserVerificationRequest request)
    {
      if (User.Identity?.Name is null)
      {
        return Forbid();
      }

      var result = await _userVerificationService.UpdateUserVerificationByUsernameAsync(
        User.Identity.Name, 
        username, 
        request.Verified);

      return result.Status switch
      {
        ResultStatus.Forbidden => Forbid(),
        ResultStatus.NotFound => NotFound(),
        ResultStatus.Ok => Ok(),
        _ => BadRequest()
      };
    }
  }
}
