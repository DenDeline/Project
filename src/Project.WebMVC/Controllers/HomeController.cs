using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using Project.WebMVC.ViewModels;

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
            var vm = new StatusViewModel();
            
            if (User.Identity.IsAuthenticated)
            {
                vm.IsUserAuthenticated = true;
                vm.AuthenticationMethod = User.FindFirstValue(ClaimTypes.AuthenticationMethod) 
                                          ?? User.FindFirstValue("amr");
                vm.Username = User.Identity.Name;
            }
            else
            {
                vm.IsUserAuthenticated = false;
            }

            vm.UserClaims = User.Claims.ToList().AsReadOnly();
            
            return View(vm);
        }
            
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
