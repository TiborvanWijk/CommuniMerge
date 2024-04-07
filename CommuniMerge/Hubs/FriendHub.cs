using CommuniMerge.ApiServices;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Hubs.ClientInterfaces;
using CommuniMerge.Library.Services;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CommuniMerge.Hubs
{
    public class FriendHub : Hub<IFriendClient>
    {
        private readonly IUserApiService userApiService;
        private readonly IAccountService accountService;

        public FriendHub(IUserApiService userApiService, IAccountService accountService)
        {
            this.userApiService = userApiService;
            this.accountService = accountService;
        }

        public async Task SendFriendRequest(string receiverUsername)
        {
            var currentlyLoggedInUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await userApiService.SendFriendRequest(Context.GetHttpContext(), receiverUsername);

            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            var sender = await accountService.GetUserByIdAsync(currentlyLoggedInUserId);
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);
            await Clients.User(receiver.Id).ReceiveFriendRequest(sender.UserName);
        }

        public async Task AcceptFriendRequest(string username)
        {
            var currentlyLoggedInUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var httpContext = Context.GetHttpContext();

            var result = await userApiService.AcceptFriendRequest(httpContext, username);

            var receiver = await accountService.GetUserByUsernameAsync(username);

            if (result.IsSuccessStatusCode)
            {
                Clients.User(receiver.Id).UpdateFriend(username);
                //Clients.User(currentlyLoggedInUserId).UpdateFriend(username);
            }
        }

        public async Task DeclineFriendRequest(string username)
        {
            var currentlyLoggedInUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var httpContext = Context.GetHttpContext();

            var result = await userApiService.DeclineFriendRequest(httpContext, username);

            var receiver = await accountService.GetUserByUsernameAsync(username);

            if (result.IsSuccessStatusCode)
            {
                Clients.User(receiver.Id).DeleteFriendRequest(username);
            }
        }
    }
}
