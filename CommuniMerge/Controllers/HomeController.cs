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
using CommuniMerge.ApiServices.Interfaces;

namespace CommuniMerge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService accountService;
        private readonly IUserApiService userApiService;

        public HomeController(ILogger<HomeController> logger, IAccountService accountService, IUserApiService userApiService)
        {
            _logger = logger;
            this.accountService = accountService;
            this.userApiService = userApiService;
        }
        [CustomAuthorize]
        public async Task<IActionResult> Index()
        {

            var friendsResult = await userApiService.GetAllFriends(HttpContext, true);
            var friendRequestsResult = await userApiService.GetAllFriendRequests(HttpContext);

            List<FriendRequestDto> friendRequest = JsonConvert.DeserializeObject<List<FriendRequestDto>>(await friendRequestsResult.Content.ReadAsStringAsync());
            List<FriendDisplayDto> friends = JsonConvert.DeserializeObject<List<FriendDisplayDto>>(await friendsResult.Content.ReadAsStringAsync());
            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await accountService.GetUserByIdAsync(id);
            return View(new IndexView() { Friends = friends, CurrentUserUsername = currentUser.UserName, FriendRequests = friendRequest });
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
