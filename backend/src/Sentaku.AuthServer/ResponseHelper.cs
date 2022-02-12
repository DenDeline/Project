using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Mvc;
using Sentaku.AuthServer.AuthServer;

namespace Sentaku.AuthServer;

public static class ResponseHelper
{
  public static IActionResult ErrorResponse(string redirectUri, string error, string? description = null, string? state = null)
  {
    Guard.Against.NullOrWhiteSpace(error, nameof(error));
    Guard.Against.NullOrWhiteSpace(redirectUri, nameof(redirectUri));
    
    OauthErrorResponse response = new(error, redirectUri)
    {
      ErrorDescription = description,
      State = state
    };
    
    return new RedirectResult(response);
  }

  public static IActionResult InvalidClient(string redirectUri, string? description = null, string? state = null)
    => ErrorResponse(redirectUri, AuthServerConstants.ErrorResponseTypes.InvalidClient, description, state);
    
  public static IActionResult InvalidRequest(string redirectUri, string? description = null, string? state = null)
    => ErrorResponse(redirectUri, AuthServerConstants.ErrorResponseTypes.InvalidRequest, description, state);
  
  public static IActionResult UnsupportedGrantType(string redirectUri, string? description = null, string? state = null)
    => ErrorResponse(redirectUri, AuthServerConstants.ErrorResponseTypes.UnsupportedGrantType, description, state);
  
  public static IActionResult InvalidGrant(string redirectUri, string? description = null, string? state = null)
    => ErrorResponse(redirectUri, AuthServerConstants.ErrorResponseTypes.InvalidGrant, description, state);
}
  
