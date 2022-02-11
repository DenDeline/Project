using Ardalis.Result;
using Sentaku.AuthServer.AuthServer.Models;

namespace Sentaku.AuthServer.AuthServer.Extensions
{
  public static class CodeTokenValidationExtensions
  {
    public static Result<bool> IsValid(
      this CodeToken codeToken,
      string clientId,
      string redirectUri,
      string codeVerifier)
    {
      if (codeToken.ExpiresIn < DateTime.UtcNow)
        return false;

      if (codeToken.ClientId != clientId)
        return false;

      if (codeToken.RedirectUri != redirectUri)
        return false;

      return codeToken.CodeChallengeMethod switch
      {
        null or AuthServerConstants.CodeChallengeMethods.Plain 
          when codeToken.CodeChallenge == codeVerifier => true,
        AuthServerConstants.CodeChallengeMethods.S256 
          when codeToken.CodeChallenge == StringEncryption.Base64UrlEncodedSha256(codeVerifier) => true,
        _ => false
      };
    }
  }
}
