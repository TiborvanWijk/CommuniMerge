using Microsoft.AspNetCore.Identity;

namespace CommuniMerge.Library.Models
{
    public class User : IdentityUser
    {
        public string ProfilePath { get; set; } = "/img/profile.jpg";
        public string About { get; set; } = "Hey I just joined CommuniMerge!";
        public ICollection<UserGroupLink> UserGroupsLinks { get; set; }
        public ICollection<UserFriend> FriendsLink { get; set; }
        public ICollection<FriendRequest> FriendRequests { get; set; }
    }
}
