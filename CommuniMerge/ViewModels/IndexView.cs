using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Models;

namespace CommuniMerge.ViewModels
{
    public class IndexView
    {
        public List<GroupDto> Groups { get; set; }
        public List<FriendDisplayDto> Friends { get; set; } = new List<FriendDisplayDto>();
        public UserDto CurrentUser { get; set; }
        public ICollection<FriendRequestDto> FriendRequests { get; set; }
    }
}
