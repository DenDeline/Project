namespace Sentaku.AuthServer.AuthServer
{
  public static class AuthServerConstants
  {
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
