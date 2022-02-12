using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Sentaku.AuthServer.AuthServer
{
  public static class StringEncryption
  {
    public static string Base64UrlEncodedSha256(string input)
    {
      using SHA256 sha256Hash = SHA256.Create();
      
      byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(input));
      return Base64UrlEncoder.Encode(bytes);
    }
  }
}
