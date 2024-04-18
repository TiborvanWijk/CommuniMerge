using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Models;

namespace Communimerge.Api.Hubs.ClientInterfaces
{
    public interface IFriendClient
    {
        Task ReceiveFriendRequest(FriendRequestDto sender);
        Task UpdateFriendListing(UserDto currentuser, UserDto friend);
        Task DeleteFriendRequestListing(string username);
        Task SuccesSendingFriendRequest(string feedbackMessage);
        Task FailSendingFriendRequest(string feedbackMessage);
    }
}
