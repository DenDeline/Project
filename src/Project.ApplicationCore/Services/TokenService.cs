using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Project.ApplicationCore.Entities;
using Project.ApplicationCore.Interfaces;

namespace Project.ApplicationCore.Services
{
    public class TokenService: ITokenService
    {

        public string CreateAccessToken(AppUser user, string signinKey)
        {
            // TODO: Remove test claim
            var claim = new Claim("lang", "ru");

            var token = new JwtSecurityToken(
                "https://localhost:44307", 
                "https://localhost:44307", 
                new []{claim}, 
                DateTime.UtcNow, 
                DateTime.UtcNow.Add(TimeSpan.FromSeconds(3600)), 
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey)), 
                    SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Result<string>.Success(encodedJwt);
        }
    }
}