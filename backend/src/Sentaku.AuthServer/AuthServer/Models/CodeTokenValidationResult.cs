namespace Sentaku.AuthServer.AuthServer.Models;

public class CodeTokenValidationResult
{
  public static CodeTokenValidationResult InvalidExpiration(DateTime expiresInDate) =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidRequest,
      ErrorDescription = $"Code token is expired in {expiresInDate:s}"
    };

  public static CodeTokenValidationResult InvalidClient() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidRequest,
      ErrorDescription = "Client doesn't exist in system"
    };
  
  public static CodeTokenValidationResult InvalidRedirectUri() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidRequest,
      ErrorDescription = "Invalid redirect uri"
    };
  
  public static CodeTokenValidationResult InvalidCodeChallengeIsNotProvided() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidRequest,
      ErrorDescription = "Code challenge required"
    };
  
  public static CodeTokenValidationResult InvalidCodeChallengeEquality() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidRequest,
      ErrorDescription = "Code challenges aren't equal"
    };
  
  public static CodeTokenValidationResult InvalidTransformAlgorithmIsNotSupported() =>
    new()
    {
      Error = AuthServerConstants.ErrorResponseTypes.InvalidRequest,
      ErrorDescription = "Transform algorithm not supported"
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
