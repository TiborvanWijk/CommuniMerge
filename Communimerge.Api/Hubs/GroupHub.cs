using Communimerge.Api.CustomAttribute;
using Communimerge.Api.Hubs.ClientInterfaces;
using Microsoft.AspNetCore.SignalR;

namespace Communimerge.Api.Hubs
{
    [CustomAuthorize]
    public class GroupHub : Hub<IGroupClient>
    {
    }
}
