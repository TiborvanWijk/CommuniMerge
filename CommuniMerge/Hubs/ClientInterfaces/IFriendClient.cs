namespace CommuniMerge.Hubs.ClientInterfaces
{
    public interface IFriendClient
    {
        Task ReceiveFriendRequest(string username);
        Task UpdateFriendListing(string currentUsersname, string friendUsername);
        Task DeleteFriendRequestListing(string username);
        Task SuccesSendingFriendRequest();
        Task FailSendingFriendRequest(string feedbackMessage);
    }
}
