using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Project.ApplicationCore.Extensions
{
    public static class StringEncryptionExtensions
    {
        public static string Sha256(this string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())  
            { 
                byte[] bytes = sha256Hash.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Convert.ToBase64String(bytes);
            } 
        }

        public static string AesEncrypt(this string plainText, string key)
        {
            var iv = new byte[16];  
            byte[] array;

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(plainText);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
        
        
        public static string AesDecrypt(this string cipherText, string key)  
        {  
            byte[] iv = new byte[16];  
            byte[] buffer = Convert.FromBase64String(cipherText);  
  
            using (Aes aes = Aes.Create())  
            {  
                aes.Key = Encoding.UTF8.GetBytes(key);  
                aes.IV = iv;  
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);  
  
                using (MemoryStream memoryStream = new MemoryStream(buffer))  
                {  
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))  
                    {  
                        using (StreamReader streamReader = new StreamReader(cryptoStream))  
                        {  
                            return streamReader.ReadToEnd();  
                        }  
                    }  
                }  
            }  
        }
    }
}