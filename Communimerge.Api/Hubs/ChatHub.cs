using Communimerge.Api.CustomAttribute;
using Communimerge.Api.Hubs.ClientInterfaces;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Communimerge.Api.Hubs
{
    [CustomAuthorize]
    public class ChatHub : Hub<IChatClient>
    {
       
    }
}
