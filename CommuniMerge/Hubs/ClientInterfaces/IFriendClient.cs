namespace CommuniMerge.Hubs.ClientInterfaces
{
    public interface IFriendClient
    {
        Task ReceiveFriendRequest(string username);
        Task UpdateFriendListing(string username);
        Task DeleteFriendRequestListing(string username);
    }
}
