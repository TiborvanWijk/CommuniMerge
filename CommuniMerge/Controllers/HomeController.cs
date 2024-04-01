using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using Communimerge.Api.CustomAttribute;
using CommuniMerge.ViewModels;
using Newtonsoft.Json;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Services.Interfaces;
using CommuniMerge.Library.Services;
using System.Net;

namespace CommuniMerge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService accountService;


        public HomeController(ILogger<HomeController> logger, IAccountService accountService)
        {
            _logger = logger;
            this.accountService = accountService;
        }
        [CustomAuthorize]
        public async Task<IActionResult> Index()
        {
            var cookieContainer = new CookieContainer();

            var handler = new HttpClientHandler { CookieContainer = cookieContainer };

            var httpClient = new HttpClient(handler);

            var cookie = Request.Cookies["BearerToken"];

            if (!string.IsNullOrEmpty(cookie))
            {
                cookieContainer.Add(new Uri("https://localhost:7129"), new Cookie("BearerToken", cookie));
            }

            var result = await httpClient.GetAsync("https://localhost:7129/api/User/friends?withLatestMessage=true");

            List<FriendDisplayDto> content = JsonConvert.DeserializeObject<List<FriendDisplayDto>>(await result.Content.ReadAsStringAsync());

            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await accountService.GetUserByIdAsync(id);
            return View(new IndexView() { Friends = content, CurrentUserUsername = currentUser.UserName });
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
