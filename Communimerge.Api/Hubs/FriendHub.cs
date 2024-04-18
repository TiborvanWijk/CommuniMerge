using Communimerge.Api.CustomAttribute;
using Communimerge.Api.Hubs.ClientInterfaces;
using CommuniMerge.ApiServices;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Library.Services;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Communimerge.Api.Hubs
{
    [CustomAuthorize]
    public class FriendHub : Hub<IFriendClient>
    {
       
    }
}
