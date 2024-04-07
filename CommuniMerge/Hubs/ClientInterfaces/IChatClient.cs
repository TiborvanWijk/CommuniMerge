namespace CommuniMerge.Hubs.ClientInterfaces
{
    public interface IChatClient
    {
        Task ReceiveMessage(string username, string message, string sendTime);
        Task ErrorSendingMessage();

    }
}
