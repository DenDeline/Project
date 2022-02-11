using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Sentaku.AuthServer.AuthServer.Extensions
{
  public static class StringEncryption
  {
    public static string Base64UrlEncodedSha256(string input)
    {
      using SHA256 sha256Hash = SHA256.Create();
      
      byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(input));
      return Base64UrlEncoder.Encode(bytes);
    }

    public static async Task<string> AesEncryptAsync(string plainText, string key)
    {
      var iv = new byte[16];

      using var aes = Aes.Create();
      aes.Key = Encoding.UTF8.GetBytes(key);
      aes.IV = iv;

      var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
      
      await using MemoryStream memoryStream = new();
      await using CryptoStream cryptoStream = new(memoryStream, encryptor, CryptoStreamMode.Write);
      
      await using (StreamWriter streamWriter = new(cryptoStream))
      {
        await streamWriter.WriteAsync(plainText);
      }

      return Convert.ToBase64String(memoryStream.ToArray());
    }


    public static async Task<string> AesDecryptAsync(string cipherText, string key)
    {
      byte[] iv = new byte[16];
      byte[] buffer = Convert.FromBase64String(cipherText);

      using Aes aes = Aes.Create();
      
      aes.Key = Encoding.UTF8.GetBytes(key);
      aes.IV = iv;
      
      var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

      await using MemoryStream memoryStream = new(buffer);

      await using CryptoStream cryptoStream = new(memoryStream, decryptor, CryptoStreamMode.Read);
      using StreamReader streamReader = new(cryptoStream);
      
      return await streamReader.ReadToEndAsync();
    }
  }
}
