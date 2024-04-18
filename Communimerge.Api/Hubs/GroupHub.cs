using Communimerge.Api.Hubs.ClientInterfaces;
using Microsoft.AspNetCore.SignalR;

namespace Communimerge.Api.Hubs
{
    public class GroupHub : Hub<IGroupClient>
    {
    }
}
