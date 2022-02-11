using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Sentaku.WebApi.Controllers
{
  public class HomeController : Controller
  {
    private readonly IWebHostEnvironment _environment;

    public HomeController(IWebHostEnvironment environment)
    {
      _environment = environment;
    }
    public IActionResult Index()
    {
      if (_environment.IsProduction())
      {
        return NotFound();
      }
      return Ok("VoteBackend project");
    }
  }
}
