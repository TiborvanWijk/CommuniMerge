namespace CommuniMerge.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(string username, string message, string sendTime);
    }
}
