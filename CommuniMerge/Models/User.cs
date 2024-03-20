﻿using Microsoft.AspNetCore.Identity;

namespace CommuniMerge.Models
{
    public class User : IdentityUser
    {
        public ICollection<UserGroupLink> UserGroupsLinks { get; set; }
        public ICollection<PrivateConversationLink> PrivateConversations { get; set; }
    }
}
