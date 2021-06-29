using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Project.WebMVC.Controllers.Api
{
    [ApiController]
    public class UserController: ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet("/api/auth")]
        public IActionResult Authorize()
        {
            var claim = new Claim("lang", "ru");
            
            var token = new JwtSecurityToken(
                "https://localhost:44307", 
                "https://localhost:44307", 
                new []{claim}, 
                DateTime.UtcNow, 
                DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)), 
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5479139e-1580-4ff9-920d-d23eb06db2ba")), 
                    SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Ok(new
            {
                access_token = encodedJwt
            });
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/api/auth/secret")]
        public IActionResult TestToken()
        {
            return Ok(new {payload = "Hello world!"});
        }
    }
}