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
using Sentaku.ApplicationCore.Interfaces;
using Sentaku.ApplicationCore.ValueObjects;
using Sentaku.Infrastructure.Data;
using Sentaku.SharedKernel;

namespace Sentaku.Infrastructure.Services
{
  public class IdentityTokenClaimService : IIdentityTokenClaimService
  {
    
    private const string DefaultTokenType = "Bearer";
    private const int DefaultTokenExpiration = 3600;
    
    private readonly UserManager<AppUser> _userManager;
    public IdentityTokenClaimService(
      UserManager<AppUser> userManager)
    {
      _userManager = userManager;
    }
    
    public async Task<Result<AuthTokenResult>> GetTokenAsync(
      string userId,
      string issuer,
      string audience,
      CancellationToken cancellationToken = default)
    {
      var userResponse = await _userManager.Users
        .Where(_ => _.Id == userId)
        .Select(_ => new { _.UserName, _.SecurityStamp })
        .FirstOrDefaultAsync(cancellationToken);

      if (userResponse is null)
        return Result<AuthTokenResult>.NotFound();
      
      var tokenHandler = new JwtSecurityTokenHandler();

      string accessToken;
      
      using (var signingIssuerCertificate = new SigningIssuerCertificate())
      {
        var key = signingIssuerCertificate.GetPrivateKey();

        var claims = new List<Claim>
        {
          new(_userManager.Options.ClaimsIdentity.UserIdClaimType, userId),
          new(_userManager.Options.ClaimsIdentity.UserNameClaimType, userResponse.UserName),
          new(_userManager.Options.ClaimsIdentity.SecurityStampClaimType, userResponse.SecurityStamp)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
          Issuer = issuer,
          Audience = audience,
          Subject = new ClaimsIdentity(claims),
          Expires = DateTime.UtcNow.AddSeconds(DefaultTokenExpiration),
          IssuedAt = DateTime.UtcNow,
          SigningCredentials = key
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        accessToken = tokenHandler.WriteToken(token);
      };

      var response = new AuthTokenResult
      {
        AccessToken = accessToken,
        TokenType = DefaultTokenType,
        ExpiresIn = DefaultTokenExpiration
      };
      
      return response;
    }
  }
}
