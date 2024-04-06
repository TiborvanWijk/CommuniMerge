namespace CommuniMerge.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string username, string message, string sendTime);
        Task ErrorSendingMessage();
        Task ReceiveFriendRequest(string username);
        Task UpdateFriend(string username);
        Task DeleteFriendRequest(string username);
    }
}
