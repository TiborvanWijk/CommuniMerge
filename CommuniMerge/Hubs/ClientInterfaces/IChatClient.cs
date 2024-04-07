namespace CommuniMerge.Hubs.ClientInterfaces
{
    public interface IChatClient
    {
        Task ReceiveMessage(string receiverUsername, string username, string message, string sendTime);
        Task ErrorSendingMessage();
    }
}
