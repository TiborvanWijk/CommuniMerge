using CommuniMerge.Library.Data.Dtos;

namespace Communimerge.Api.Hubs.ClientInterfaces
{
    public interface IChatClient
    {
        Task ReceiveMessage(string receiverUsername, MessageDisplayDto messageDisplayDto);
        Task ErrorSendingMessage();
        Task ReceiveGroupMessage(int groupId, MessageDisplayDto messageDisplayDto);
    }
}
