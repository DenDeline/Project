using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Project.ApplicationCore.Entities;
using Project.ApplicationCore.Extensions;
using Project.WebMVC.ViewModels;

namespace Project.WebMVC.Controllers
{
    public class OauthController: Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public OauthController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        
        [HttpGet("/oauth2/authorize")]
        public IActionResult Authorize(
            [FromQuery] string response_type,
            [FromQuery] string client_id,
            [FromQuery] string redirect_uri,
            [FromQuery] string state,
            [FromQuery] string code_challenge,
            [FromQuery] string code_challenge_method)
        {
            var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == client_id);
            
            // TODO: Separate class for validation required params 
            if (client is null)
            {
                return BadRequest();
            }

            if (!client.RedirectUris.Contains(redirect_uri))
            {
                //TODO: Add error validation constant: invalid_request
                var queryBuilder = new QueryBuilder
                {
                    {"error", "invalid_request"},
                    {"state", state}
                };
                return Redirect($"{redirect_uri}{queryBuilder}");
            }

            if (AuthServerConfig.SupportedResponseTypes.All(_ => _ != response_type))
            {
                //TODO: Add error validation constant: unsupported_response_type
                var queryBuilder = new QueryBuilder
                {
                    {"error", "unsupported_response_type"},
                    {"state", state}
                };
                return Redirect($"{redirect_uri}{queryBuilder}");
            }

            var vm = new AuthorizeViewModel
            {
                ClientId = client_id,
                RedirectUri = redirect_uri,
                State = state,
                CodeChallenge = code_challenge,
                CodeChallengeMethod = code_challenge_method
            };
            
            return View(vm);
        }

        [HttpPost("/oauth2/signin-code")]
        public async Task<IActionResult> SignInCode(AuthorizeViewModel vm)
        {
            var user = (await _userManager.FindByEmailAsync(vm.Login)) ?? (await  _userManager.FindByNameAsync(vm.Login));

            if (user is null)
            {
                // TODO: Add validation error and append query params
                ModelState.AddModelError(vm.Login, "");
                return View("Authorize", vm);
            }

            var isPasswordValid =  await _userManager.CheckPasswordAsync(user, vm.Password);

            if (!isPasswordValid)
            {
                // TODO: Add validation error and append query params
                ModelState.AddModelError(vm.Password, "");
                return View("Authorize", vm);
            }
            
            
            // TODO: Move out code token generation
            var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == vm.ClientId);
            
            if (client is null)
            {
                // TODO: Add validation error and append query params
                ModelState.AddModelError("", "");
                return View("Authorize", vm);
            }

            var codeToken = new CodeToken
            {
                ClientId = vm.ClientId,
                RedirectUri = vm.RedirectUri,
                UserId = user.Id,
                CodeChallenge = vm.CodeChallenge,
                CodeChallengeMethod = vm.CodeChallengeMethod
            };

            var jsonCodeToken = JsonConvert.SerializeObject(codeToken);

            var encodedCodeToken = jsonCodeToken.AesEncrypt(client.ClientSecret);

            // TODO: Create separate class for building redirectUrl
            var queryBuilder = new QueryBuilder
            {
                {"code", encodedCodeToken}, 
                {"state", vm.State}
            };

            var redirectUrl = $"{client.RedirectUris.FirstOrDefault()}{queryBuilder}";
            return Redirect(redirectUrl);
        }


        [HttpPost("/oauth2/token")]
        public async Task<IActionResult> Token([FromBody] TokenRequest vm, [FromServices] IConfiguration configuration)
        {
            if (AuthServerConfig.SupportedGrantTypes.All(_ => _ != vm.grant_type))
            {
                // TODO: Add validation error
                return BadRequest();
            };
            
            var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == vm.client_id);
            if (client is null)
            {
                // TODO: Add validation error
                return BadRequest();
            }

            var decodedCodeToken = vm.code.AesDecrypt(client.ClientSecret);
            var codeToken = JsonConvert.DeserializeObject<CodeToken>(decodedCodeToken);

            if (!codeToken.IsValid(vm.client_id, vm.redirect_uri, "S256", vm.code_verifier))
            {
                return BadRequest();
            };

            var user = await _userManager.FindByIdAsync(codeToken.UserId);
            
            if (user is null)
            {
                // TODO: Add validation error
                return BadRequest();
            }
            
            // TODO: Move out jwt generation
            var claim = new Claim("lang", "ru");

            var token = new JwtSecurityToken(
                "https://localhost:44307", 
                "https://localhost:44307", 
                new []{claim}, 
                DateTime.UtcNow, 
                DateTime.UtcNow.Add(TimeSpan.FromSeconds(3600)), 
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:Project:ClientSecret"])), 
                    SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Ok(new
            {
                access_token = encodedJwt,
                token_type = "bearer",
                expires_in = 3600
            });
        }
    }

    // TODO: Move to separate file
    public class TokenRequest
    {
        public string grant_type { get; set; }
        public string code { get; set; }
        public string redirect_uri { get; set; }
        public string client_id { get; set; }
        public string code_verifier { get; set; }
    }
}