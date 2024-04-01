using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Communimerge.Api.CustomAttribute;

namespace CommuniMerge.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient client;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7129");
            _logger = logger;
        }
        [CustomAuthorize]
        public IActionResult Index()
        {

            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        public IActionResult Privacy()
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
