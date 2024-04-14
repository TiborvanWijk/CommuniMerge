using Communimerge.Api.CustomAttribute;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Hubs.ClientInterfaces;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Security.Claims;

namespace CommuniMerge.Hubs
{
    [CustomAuthorize]
    public class ChatHub : Hub<IChatClient>
    {
        private readonly IApiService apiService;
        private readonly IAccountService accountService;

        public ChatHub(IApiService apiService, IAccountService accountService)
        {
            this.apiService = apiService;
            this.accountService = accountService;
        }


        public async Task SendGroupMessage(int groupId, string message)
        {
            var id = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await accountService.GetUserByIdAsync(id);
            HttpContext context = Context.GetHttpContext();
            var groupMessageCreateDto = new GroupMessageCreateDto
            {
                GroupId = groupId,
                Content = message
            };

            var result = await apiService.SendHttpRequest<GroupMessageCreateDto>
                (Context.GetHttpContext(), $"/api/Message/CreateGroupMessage", HttpMethod.Post, groupMessageCreateDto);

            if(!result.IsSuccessStatusCode)
            {
                return;
            }
            var groupMembersResult = await apiService.SendHttpRequest<object?>
                (Context.GetHttpContext(), $"/api/Group/getMembers/{groupId}", HttpMethod.Get, null);

            var members = JsonConvert.DeserializeObject<ICollection<FriendDto>>(await groupMembersResult.Content.ReadAsStringAsync());

            foreach (var member in members)
            {
                Clients.User(member.Id).ReceiveGroupMessage(groupId, user.UserName, message, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            }

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
            var result = await apiService.SendHttpRequest<PersonalMessageCreateDto>(context, "/api/Message/createPersonalMessage", HttpMethod.Post, personalMessageCreateDto);
            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);

            await Clients.User(receiver.Id).ReceiveMessage(receiverUsername, user.UserName, message, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
            await Clients.User(id).ReceiveMessage(receiverUsername, user.UserName, message, DateTime.Now.ToString("MM/dd/yyyy hh:mm tt"));
        }
    }
}
