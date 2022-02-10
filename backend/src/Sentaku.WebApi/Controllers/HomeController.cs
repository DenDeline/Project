using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Sentaku.WebApi.ViewModels;

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


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
