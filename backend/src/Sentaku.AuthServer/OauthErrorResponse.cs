using Microsoft.AspNetCore.Http.Extensions;
using Sentaku.AuthServer.AuthServer;

namespace Sentaku.AuthServer;

public class OauthErrorResponse
{
  public OauthErrorResponse(string error, string redirectUri)
  {
    Error = error;
    RedirectUri = redirectUri;
  }
  public string RedirectUri { get; }
  public string Error { get; }
  public string? ErrorDescription { get; init; }
  public string? State { get; init; }

  public static implicit operator string(OauthErrorResponse response) =>
    response.ToString();
  
  public override string ToString()
  {
    var queryBuilder = new QueryBuilder
    {
      { "error", Error }
    };
    
    if (ErrorDescription is not null)
      queryBuilder.Add("error_description", ErrorDescription);
    
    if (State is not null)
      queryBuilder.Add("state", State);

    return $"{RedirectUri}{queryBuilder}";
  }
}
