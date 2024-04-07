namespace CommuniMerge.Hubs.ClientInterfaces
{
    public interface IFriendClient
    {
        Task ReceiveFriendRequest(string username);
        Task UpdateFriend(string username);
        Task DeleteFriendRequest(string username);
    }
}
