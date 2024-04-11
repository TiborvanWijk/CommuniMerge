namespace CommuniMerge.Hubs.ClientInterfaces
{
    public interface IFriendClient
    {
        Task ReceiveFriendRequest(string username);
        Task UpdateFriendListing(string currentUsersname, string friendUsername);
        Task DeleteFriendRequestListing(string username);
        Task SuccesSendingFriendRequest(string feedbackMessage);
        Task FailSendingFriendRequest(string feedbackMessage);
    }
}
