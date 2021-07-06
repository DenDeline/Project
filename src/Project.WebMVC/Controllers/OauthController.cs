using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Project.WebMVC.ViewModels;

namespace Project.WebMVC.Controllers
{
    public class OauthController: Controller
    {
        [HttpGet("/authorize")]
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

            var vm = new AuthorizeViewModel();
            
            return View(vm);
        }


        [HttpPost]
        public IActionResult Token(AuthorizeViewModel vm)
        {
            return Ok();
        }
    }
}