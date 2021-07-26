using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Result;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.ApplicationCore.Entities;
using Project.ApplicationCore.Interfaces;

namespace Project.ApplicationCore.Services
{
    public class TokenService: ITokenService
    {
        private readonly IDbContext _dbContext;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IDbContext dbContext, UserManager<AppUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<Result<string>> CreateAccessTokenAsync(
            int userId, 
            SigningCredentials signinKey, 
            CancellationToken ctsToken = new CancellationToken())
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return Result<string>.NotFound();
            }

            var language = await _dbContext.Languages.FirstOrDefaultAsync(_ => _.Id == user.LanguageId, ctsToken);
            
            if (language is null)
            {
                return Result<string>.NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userClaims = new List<Claim>
            {
                new Claim("lang", language.Code),
                new Claim("username", user.UserName)
            };

            userClaims.AddRange(roles.Select(_ => new Claim("role", _)));
            
            var token = new JwtSecurityToken(
                "https://localhost:44307", 
                "https://localhost:44307", 
                userClaims.AsReadOnly(), 
                DateTime.UtcNow, 
                DateTime.UtcNow.Add(TimeSpan.FromSeconds(3600)), signinKey);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Result<string>.Success(encodedJwt);
        }
    }
}