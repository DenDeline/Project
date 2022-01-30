using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace Project.SharedKernel
{
    public class SigningIssuerCertificate: IDisposable
    {
        private readonly RSA _rsa = RSA.Create();
        
        public RsaSecurityKey GetPublicKey()
        {
            _rsa.FromXmlString(File.ReadAllText("../../public_key.xml"));
            return new RsaSecurityKey(_rsa);
        }

        public SigningCredentials GetPrivateKey()
        {
            _rsa.FromXmlString(File.ReadAllText("../../private_key.xml"));
            return new SigningCredentials(new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaSha256);
        }

        public void Dispose()
        {
          _rsa.Dispose();
        }
    }
}
