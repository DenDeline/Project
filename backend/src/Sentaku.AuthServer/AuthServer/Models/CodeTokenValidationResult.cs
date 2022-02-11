namespace Sentaku.AuthServer.AuthServer.Models;

public class CodeTokenValidationResult
{
  public static CodeTokenValidationResult InvalidExpiration(DateTime expiresInDate) =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidGrant,
      ErrorDescription = $"Code token is expired in {expiresInDate:s}"
    };

  public static CodeTokenValidationResult InvalidClient() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidGrant,
      ErrorDescription = "Client doesn't exist in system"
    };
  
  public static CodeTokenValidationResult InvalidRedirectUri() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidGrant,
      ErrorDescription = "Invalid redirect uri"
    };
  
  public static CodeTokenValidationResult InvalidCodeChallenge() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidGrant,
      ErrorDescription = "Invalid code challenge"
    };
  public static CodeTokenValidationResult Valid() =>
    new()
    {
      IsValid = true
    };
  
  public bool IsValid { get; set; }
  public string Error { get; set; }
  public string ErrorDescription { get; set; }
}
