using Microsoft.AspNetCore.SignalR;

namespace CommuniMerge.Hubs
{
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(string user, string message)
        {
            Clients.All.ReceiveMessage(user, message, DateTime.Now.ToShortDateString());
        }
    }
}
