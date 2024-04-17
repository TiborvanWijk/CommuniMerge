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


        public async Task SendGroupMessage(GroupMessageCreateDto groupMessageCreateDto)
        {
            var id = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await accountService.GetUserByIdAsync(id);
            HttpContext context = Context.GetHttpContext();

            var result = await apiService.SendHttpRequest<GroupMessageCreateDto>
                (Context.GetHttpContext(), $"/api/Message/CreateGroupMessage", HttpMethod.Post, groupMessageCreateDto, true);

            if(!result.IsSuccessStatusCode)
            {
                return;
            }
            var groupMembersResult = await apiService.SendHttpRequest<object?>
                (Context.GetHttpContext(), $"/api/Group/getMembers/{groupMessageCreateDto.GroupId}", HttpMethod.Get, null);

            var members = JsonConvert.DeserializeObject<ICollection<UserDto>>(await groupMembersResult.Content.ReadAsStringAsync());

            foreach (var member in members)
            {
                Clients.User(member.Id).ReceiveGroupMessage(groupMessageCreateDto.GroupId, user.UserName, groupMessageCreateDto.Content, DateTime.Now.ToString());
            }

        }

        public async Task SendMessage(PersonalMessageCreateDto personalMessageCreateDto)
        {

            var id = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await accountService.GetUserByIdAsync(id);
            HttpContext context = Context.GetHttpContext();

            var result = await apiService.SendHttpRequest<PersonalMessageCreateDto>(context, "/api/Message/createPersonalMessage", HttpMethod.Post, personalMessageCreateDto, true);
            if (!result.IsSuccessStatusCode)
            {
                return;
            }
            var receiver = await accountService.GetUserByUsernameAsync(personalMessageCreateDto.ReceiverUsername);

            await Clients.User(receiver.Id).ReceiveMessage(personalMessageCreateDto.ReceiverUsername, user.UserName, personalMessageCreateDto.Content, DateTime.Now.ToString());
            await Clients.User(id).ReceiveMessage(personalMessageCreateDto.ReceiverUsername, user.UserName, personalMessageCreateDto.Content, DateTime.Now.ToString());
        }
    }
}
