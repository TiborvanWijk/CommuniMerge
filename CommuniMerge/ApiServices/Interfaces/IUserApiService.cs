namespace CommuniMerge.ApiServices.Interfaces
{
    public interface IUserApiService
    {
        Task<HttpResponseMessage> SendFriendRequest(HttpContext httpContext, string receiverUsername);
    }
}
