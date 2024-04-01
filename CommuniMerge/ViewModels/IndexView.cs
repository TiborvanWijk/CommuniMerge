using CommuniMerge.Library.Data.Dtos;

namespace CommuniMerge.ViewModels
{
    public class IndexView
    {
        public ICollection<FriendDisplayDto> Friends { get; set; } = new List<FriendDisplayDto>();
        public string CurrentUserUsername { get; set; }
    }
}
