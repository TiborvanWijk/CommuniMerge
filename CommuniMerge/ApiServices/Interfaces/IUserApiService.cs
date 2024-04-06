using CommuniMerge.Library.Models;

namespace CommuniMerge.ApiServices.Interfaces
{
    public interface IUserApiService
    {
        Task<HttpResponseMessage> SendFriendRequest(HttpContext httpContext, string receiverUsername);
        Task<HttpResponseMessage> GetAllFriends(HttpContext httpContext, bool withLatestMessage);
        Task<HttpResponseMessage> GetAllFriendRequests(HttpContext httpContext);
        Task<HttpResponseMessage> AcceptFriendRequest(HttpContext httpContext, string username);
        Task<HttpResponseMessage> DeclineFriendRequest(HttpContext httpContext, string username);

    }
}
