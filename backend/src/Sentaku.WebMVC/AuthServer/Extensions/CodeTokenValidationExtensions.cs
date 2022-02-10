using System;
using Ardalis.Result;
using Project.WebMVC.AuthServer.Models;

namespace Project.WebMVC.AuthServer.Extensions
{
  public static class CodeTokenValidationExtensions
  {
    public static Result<bool> IsValid(
      this CodeToken codeToken,
      string clientId,
      string redirectUri,
      string codeVerifier)
    {
      if (codeToken.ExpiresIn < DateTime.UtcNow) return false;

      if (codeToken.ClientId != clientId) return false;

      if (codeToken.RedirectUri != redirectUri) return false;

      if (codeToken.CodeChallengeMethod is null || codeToken.CodeChallengeMethod == "plain")
      {
        if (codeToken.CodeChallenge != codeVerifier) return false;
      }
      else if (codeToken.CodeChallengeMethod == "S256")
      {
        if (codeToken.CodeChallenge != codeVerifier.Sha256()) return false;
      }
      else return false;

      return true;
    }
  }
}
