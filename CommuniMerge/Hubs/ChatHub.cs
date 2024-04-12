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
        private readonly IApiService apiService;
        private readonly IAccountService accountService;

        //private readonly IAccountService accountService;
        //private readonly IMessageApiService messageApiService;
        //private readonly IUserApiService userApiService;

        //public ChatHub(IAccountService accountService, IMessageApiService messageApiService, IUserApiService userApiService)
        //{
        //    this.accountService = accountService;
        //    this.messageApiService = messageApiService;
        //    this.userApiService = userApiService;
        //}
        public ChatHub(IApiService apiService, IAccountService accountService)
        {
            this.apiService = apiService;
            this.accountService = accountService;
        }


        public async Task SendMessage(string receiverUsername, string message)
        {

            var id = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await accountService.GetUserByIdAsync(id);
            HttpContext context = Context.GetHttpContext();
            var personalMessageCreateDto = new PersonalMessageCreateDto 
            {
                ReceiverUsername = receiverUsername, Content = message 
            };
            //var result = await messageApiService.CreatePersonalMessage(context, "", );
            var result = await apiService.SendHttpRequest<PersonalMessageCreateDto>(context, "/api/Message/createPersonalMessage", HttpMethod.Post, personalMessageCreateDto);
            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);

            await Clients.User(receiver.Id).ReceiveMessage(receiverUsername, user.UserName, message, DateTime.Now.ToShortDateString());
            await Clients.User(id).ReceiveMessage(receiverUsername, user.UserName, message, DateTime.Now.ToShortDateString());
        }
    }
}
