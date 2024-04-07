using Communimerge.Api.CustomAttribute;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Hubs.ClientInterfaces;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CommuniMerge.Hubs
{
    [CustomAuthorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IAccountService accountService;
        private readonly IMessageApiService messageApiService;
        private readonly IUserApiService userApiService;

        public ChatHub(IAccountService accountService, IMessageApiService messageApiService, IUserApiService userApiService)
        {
            this.accountService = accountService;
            this.messageApiService = messageApiService;
            this.userApiService = userApiService;
        }

        

        public async Task SendMessage(string receiverUsername, string message)
        {

            var id = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await accountService.GetUserByIdAsync(id);
            HttpContext context = Context.GetHttpContext();

            var result = await messageApiService.CreatePersonalMessage(context, new PersonalMessageCreateDto { ReceiverUsername = receiverUsername, Content = message });

            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);

            await Clients.User(receiver.Id).ReceiveMessage(user.UserName, message, DateTime.Now.ToShortDateString());
            await Clients.User(id).ReceiveMessage(user.UserName, message, DateTime.Now.ToShortDateString());
        }
    }
}
