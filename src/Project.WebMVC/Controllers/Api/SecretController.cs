using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project.WebMVC.Controllers.Api
{
    [ApiController]
    public class SecretController: ControllerBase
    {
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("/api/auth/secret")]
        public IActionResult TestToken()
        {
            return Ok(new {payload = "Hello world!"});
        }
    }
}