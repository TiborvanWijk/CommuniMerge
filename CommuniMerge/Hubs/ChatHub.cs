using Communimerge.Api.CustomAttribute;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace CommuniMerge.Hubs
{
    [CustomAuthorize]
    public class ChatHub : Hub<IChatClient>
    {
        private HttpClient client { get; }

        public ChatHub()
        {
            client = new HttpClient();
        }


        public async Task SendMessage(string user, string message)
        {
            Clients.All.ReceiveMessage(user, message, DateTime.Now.ToShortDateString());
            var id = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
