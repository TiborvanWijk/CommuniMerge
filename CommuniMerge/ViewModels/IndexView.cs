using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Models;

namespace CommuniMerge.ViewModels
{
    public class IndexView
    {
        public ICollection<GroupDto> Groups { get; set; }
        public ICollection<FriendDisplayDto> Friends { get; set; } = new List<FriendDisplayDto>();
        public string CurrentUserUsername { get; set; }
        public ICollection<FriendRequestDto> FriendRequests { get; set; }
    }
}
