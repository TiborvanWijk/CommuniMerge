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
using System.Collections.Generic;

namespace CommuniMerge.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAccountService accountService;
        private readonly IApiService apiService;

        public HomeController(ILogger<HomeController> logger, IAccountService accountService, IApiService apiService)
        {
            _logger = logger;
            this.accountService = accountService;
            this.apiService = apiService;
        }
        [CustomAuthorize]
        public async Task<IActionResult> Index()
        {

            var friendsResult = await apiService.SendHttpRequest<object>(HttpContext, "/api/User/friends?withLatestMessage=true", HttpMethod.Get, null);
            var friendRequestsResult = await apiService.SendHttpRequest<object>(HttpContext, "/api/User/friendRequests", HttpMethod.Get, null);
            var groupsResult = await apiService.SendHttpRequest<object>(HttpContext, "/api/Group/getGroups?withLatestMessage=true", HttpMethod.Get, null);

            List<FriendDisplayDto> friends = JsonConvert.DeserializeObject<List<FriendDisplayDto>>(await friendsResult.Content.ReadAsStringAsync());
            List < FriendRequestDto > friendRequest = JsonConvert.DeserializeObject<List<FriendRequestDto>>(await friendRequestsResult.Content.ReadAsStringAsync());
            List<GroupDto> groups = JsonConvert.DeserializeObject<List<GroupDto>>(await groupsResult.Content.ReadAsStringAsync());

            var id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUser = await accountService.GetUserByIdAsync(id);

            var indexView = new IndexView()
            {
                Friends = friends,
                CurrentUserUsername = currentUser.UserName,
                FriendRequests = friendRequest,
                Groups = groups
            };

            return View(indexView);
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
