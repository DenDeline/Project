namespace Sentaku.AuthServer.AuthServer
{
  public static class AuthServerConstants
  {
    public static class ErrorResponseTypes
    {
      /// <summary>
      /// The request is missing a required parameter, includes an
      /// unsupported parameter value (other than grant type),
      /// repeats a parameter, includes multiple credentials,
      /// utilizes more than one mechanism for authenticating the
      /// client, or is otherwise malformed
      /// </summary>
      public const string InvalidRequest = "invalid_request";
      
      /// <summary>
      /// Client authentication failed (e.g., unknown client, no
      /// client authentication included, or unsupported
      /// authentication method).  The authorization server MAY
      /// return an HTTP 401 (Unauthorized) status code to indicate
      /// which HTTP authentication schemes are supported.  If the
      /// client attempted to authenticate via the "Authorization"
      /// request header field, the authorization server MUST
      /// respond with an HTTP 401 (Unauthorized) status code and
      /// include the "WWW-Authenticate" response header field
      /// matching the authentication scheme used by the client.
      /// </summary>
      public const string InvalidClient = "invalid_client";
      
      /// <summary>
      /// The provided authorization grant (e.g., authorization
      /// code, resource owner credentials) or refresh token is
      /// invalid, expired, revoked, does not match the redirection
      /// URI used in the authorization request, or was issued to
      /// another client.
      /// </summary>
      public const string InvalidGrant = "invalid_grant";
      
      /// <summary>
      /// The authenticated client is not authorized to use this authorization grant type.
      /// </summary>
      public const string UnauthorizedClient = "unauthorized_client";
      
      /// <summary>
      /// The authorization grant type is not supported by the authorization server.
      /// </summary>
      public const string UnsupportedGrantType = "unsupported_grant_type";
      
      /// <summary>
      /// The requested scope is invalid, unknown, malformed, or
      /// exceeds the scope granted by the resource owner.
      /// </summary>
      public const string InvalidScope = "invalid_scope";

      /// <summary>
      /// The resource owner or authorization server denied the request.
      /// </summary>
      public const string AccessDenied = "access_denied";

      /// <summary>
      /// The authorization server does not support obtaining an authorization code using this method.
      /// </summary>
      public const string UnsupportedResponseType = "unsupported_response_type";

      /// <summary>
      /// The authorization server encountered an unexpected
      /// condition that prevented it from fulfilling the request.
      /// (This error code is needed because a 500 Internal Server
      /// Error HTTP status code cannot be returned to the client
      /// via an HTTP redirect.)
      /// </summary>
      public const string ServerError = "server_error";
      
      /// <summary>
      /// The authorization server is currently unable to handle
      /// the request due to a temporary overloading or maintenance
      /// of the server.  (This error code is needed because a 503
      /// Service Unavailable HTTP status code cannot be returned
      /// to the client via an HTTP redirect.)
      /// </summary>
      public const string TemporarilyUnavailable = "temporarily_unavailable";
      
      /// <summary>
      /// The resource owner doesn't exist
      /// </summary>
      public const string ResourceOwnerNotFound = "resource_owner_not_found";
    }
    public static class CodeChallengeMethods
    {
      public const string Plain = "plain";
      public const string S256 = "S256";
    }
    public static class ResponseTypes
    {
      public const string Code = "code";
    }

    public static class GrantTypes
    {
      public const string AuthorizationCode = "authorization_code";
    }
  }
}
