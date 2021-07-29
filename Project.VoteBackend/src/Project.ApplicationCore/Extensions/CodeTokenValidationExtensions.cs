using System;
using Ardalis.Result;
using Project.ApplicationCore.Entities;

namespace Project.ApplicationCore.Extensions
{
    public static class CodeTokenValidationExtensions
    {
        public static Result<bool> IsValid(
            this CodeToken codeToken, 
            string clientId,
            string redirectUri,
            string codeChallengeMethod, 
            string codeVerifier)
        {
            if (codeToken is null)  
            {
                return false;
            }

            if (codeToken.ExpiresIn < DateTime.UtcNow)
            {
                return false;
            }

            if (codeToken.ClientId != clientId)
            {
                return false;
            }
            
            if (codeToken.RedirectUri != redirectUri)
            {
                return false;
            }

            if (codeToken.CodeChallengeMethod != codeChallengeMethod)
            {
                return false;
            }

            if (codeToken.CodeChallenge != codeVerifier.Sha256())
            {
                return false;
            }

            return true;
        }
    }
}