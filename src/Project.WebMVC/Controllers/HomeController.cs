using Microsoft.AspNetCore.Mvc;
using Project.WebMVC.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace Project.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Status()
        {
            return View();
        }
            
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
