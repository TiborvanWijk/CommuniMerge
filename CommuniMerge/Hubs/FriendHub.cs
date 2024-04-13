using Communimerge.Api.CustomAttribute;
using CommuniMerge.ApiServices;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Hubs.ClientInterfaces;
using CommuniMerge.Library.Services;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CommuniMerge.Hubs
{
    [CustomAuthorize]
    public class FriendHub : Hub<IFriendClient>
    {
        private readonly IAccountService accountService;
        private readonly IApiService apiService;

        public FriendHub(IAccountService accountService, IApiService apiService)
        {
            this.accountService = accountService;
            this.apiService = apiService;
        }

        public async Task SendFriendRequest(string receiverUsername)
        {
            var currentlyLoggedInUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await apiService.SendHttpRequest<object?>(Context.GetHttpContext(), $"/api/User/sendFriendRequest/{receiverUsername}", HttpMethod.Post, null);

            if (!result.IsSuccessStatusCode)
            {
                
                
                //Maybe use the api response 
                string message = "Check if the username is correct.";
                await Clients.User(currentlyLoggedInUserId).FailSendingFriendRequest(message);
                
                return;
            }
            string feedbackMessage = "Friend request succesfully sent.";
            var sender = await accountService.GetUserByIdAsync(currentlyLoggedInUserId);
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);
            await Clients.User(receiver.Id).ReceiveFriendRequest(sender.UserName);
            await Clients.User(currentlyLoggedInUserId).SuccesSendingFriendRequest(feedbackMessage);
        }

        public async Task AcceptFriendRequest(string senderUsername)
        {
            var receiverId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var httpContext = Context.GetHttpContext();

            var result = await apiService.SendHttpRequest<object?>(Context.GetHttpContext(), $"/api/User/acceptFriendRequest/{senderUsername}", HttpMethod.Post, null);

            var sender = await accountService.GetUserByUsernameAsync(senderUsername);
            var receiver = await accountService.GetUserByIdAsync(receiverId);
            if (result.IsSuccessStatusCode)
            {
                await Clients.User(receiverId).DeleteFriendRequestListing(senderUsername);
                await Clients.User(sender.Id).UpdateFriendListing(sender.UserName, receiver.UserName);
                await Clients.User(receiverId).UpdateFriendListing(receiver.UserName, sender.UserName);
            }
        }

        public async Task DeclineFriendRequest(string senderUsername)
        {
            var currentlyLoggedInUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var httpContext = Context.GetHttpContext();

            var result = await apiService.SendHttpRequest<object?>(Context.GetHttpContext(), $"/api/User/declineFriendRequest/{senderUsername}", HttpMethod.Post, null);

            if (result.IsSuccessStatusCode)
            {
                await Clients.User(currentlyLoggedInUserId).DeleteFriendRequestListing(senderUsername);
            }
        }
    }
}
