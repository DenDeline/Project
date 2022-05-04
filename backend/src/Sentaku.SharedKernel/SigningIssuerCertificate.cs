using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Sentaku.SharedKernel
{
  public class SigningIssuerCertificate: IDisposable
  {
    private readonly ECDsa _ecdsa;
    
    public SigningIssuerCertificate()
    {
      _ecdsa = ECDsa.Create();
    }
    public ECDsaSecurityKey GetPublicKey()
    {
      _ecdsa.ImportFromPem(File.ReadAllText("public-key.pem"));
      return new ECDsaSecurityKey(_ecdsa);
    }

    public SigningCredentials GetPrivateKey()
    {
      _ecdsa.ImportFromPem(File.ReadAllText("private-key.pem"));

      return new SigningCredentials(new ECDsaSecurityKey(_ecdsa), SecurityAlgorithms.EcdsaSha256)
      {
        CryptoProviderFactory = new CryptoProviderFactory
        {
          CacheSignatureProviders = false
        }
      };
    }

    public void Dispose()
    {
      _ecdsa.Dispose();
    }
  }
}
