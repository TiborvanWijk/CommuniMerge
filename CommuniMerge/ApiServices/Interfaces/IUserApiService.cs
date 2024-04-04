using CommuniMerge.Library.Models;

namespace CommuniMerge.ApiServices.Interfaces
{
    public interface IUserApiService
    {
        Task<HttpResponseMessage> SendFriendRequest(HttpContext httpContext, string receiverUsername);
        Task<HttpResponseMessage> GetAllFriends(HttpContext httpContext, bool withLatestMessage);
        Task<HttpResponseMessage> GetAllFriendRequests(HttpContext httpContext);
    }
}
