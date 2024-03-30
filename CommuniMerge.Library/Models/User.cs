using Microsoft.AspNetCore.Identity;

namespace CommuniMerge.Library.Models
{
    public class User : IdentityUser
    {
        public ICollection<UserGroupLink> UserGroupsLinks { get; set; }
        public ICollection<UserFriend> FriendsLink { get; set; }

    }
}
