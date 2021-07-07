using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Project.Infrastructure.Data;
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
            if (AuthServerConfig.SupportedResponseTypes.All(_ => _ != response_type))
            {
                // TODO: Implement other types for scaling
                return BadRequest();
            }
            
            var client = AuthServerConfig.InMemoryClients.FirstOrDefault(_ => _.ClientId == client_id);
            
            if (client is null)
            {
                // TODO: Implement for foreign clients
                return BadRequest();
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

        [HttpPost("/oauth2/validate-login")]
        public async Task<IActionResult> ValidateLogin(string login)
        {
            var user = (await _userManager.FindByEmailAsync(login)) ?? (await  _userManager.FindByNameAsync(login));
            if (user is null)
            {
                return BadRequest();
            }
            return Ok();
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
            
            
            // TODO: Move out code generation
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

            var encodedCodeToken = EncryptString(client.ClientSecret, jsonCodeToken);

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
        public async Task<IActionResult> Token([FromBody] TokenRequest vm)
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

            var decodedCodeToken = DecryptString(client.ClientSecret, vm.code);
            var codeToken = JsonConvert.DeserializeObject<CodeToken>(decodedCodeToken);

            
            // TODO: Move code token validation to the separate class
            if (codeToken is null)
            {
                // TODO: Add validation error
                return BadRequest();
            }

            if (codeToken.ExpireAt < DateTime.UtcNow)
            {
                // TODO: Add validation error
                return BadRequest();
            }

            if (codeToken.ClientId != vm.client_id)
            {
                // TODO: Add validation error
                return BadRequest();
            }
            
            if (codeToken.RedirectUri != vm.redirect_uri)
            {
                // TODO: Add validation error
                return BadRequest();
            }

            if (codeToken.CodeChallengeMethod != "S256")
            {
                // TODO: Add validation error
                return BadRequest();
            }

            if (vm.code_verifier.Sha256() != codeToken.CodeChallenge)
            {
                // TODO: Add validation error
                return BadRequest();
            }

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
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5479139e-1580-4ff9-920d-d23eb06db2ba")), 
                    SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return Ok(new
            {
                access_token = encodedJwt,
                token_type = "bearer",
                expires_in = 3600
            });
        }
        
        // TODO: Move to separate encryption class
        [NonAction]
        private string EncryptString(string key, string plainText)
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
        
        
        // TODO: Move to separate encryption class
        [NonAction]
        public static string DecryptString(string key, string cipherText)  
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
        
        // TODO: Move to models folder
        class CodeToken
        {
            public Guid Id { get; } = Guid.NewGuid();
            public string ClientId { get; set; }
            public string UserId { get; set; }
            public DateTime ExpireAt { get; } = DateTime.UtcNow.AddSeconds(60);
            public string RedirectUri { get; set; }
            public string CodeChallenge { get; set; }
            public string CodeChallengeMethod { get; set; }
        }
    }

    // TODO: Move to separate class
    public static class StringEncryptionExtensions
    {
        public static string Sha256(this string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())  
            {  
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));  
  
                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();  
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }  
                return builder.ToString();  
            } 
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