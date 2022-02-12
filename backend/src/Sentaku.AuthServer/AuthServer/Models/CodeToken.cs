namespace Sentaku.AuthServer.AuthServer.Models
{
  public record CodeToken
  {
    public Guid Id { get; } = Guid.NewGuid();
    public string ClientId { get; init; } = default!;
    public string UserId { get; init; } = default!;
    public DateTime ExpiresIn { get; } = DateTime.UtcNow.AddSeconds(60);
    public string RedirectUri { get; init; } = default!;
    public string? CodeChallenge { get; init; }
    public string? CodeChallengeMethod { get; init; }
    
    public CodeTokenValidationResult Validate(
      string clientId,
      string redirectUri,
      string codeVerifier)
    {
      if (ExpiresIn < DateTime.UtcNow)
        return CodeTokenValidationResult.InvalidExpiration(ExpiresIn);

      if (ClientId != clientId)
        return CodeTokenValidationResult.InvalidClient();

      if (RedirectUri != redirectUri)
        return CodeTokenValidationResult.InvalidRedirectUri();

      return CodeChallengeMethod switch
      {
        null or AuthServerConstants.CodeChallengeMethods.Plain 
          when CodeChallenge == codeVerifier => CodeTokenValidationResult.Valid(),
        AuthServerConstants.CodeChallengeMethods.S256 
          when CodeChallenge == StringEncryption.Base64UrlEncodedSha256(codeVerifier) => CodeTokenValidationResult.Valid(),
        _ => CodeTokenValidationResult.InvalidCodeChallengeEquality()
      };
    }
  }
}
