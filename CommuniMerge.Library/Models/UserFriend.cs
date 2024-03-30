using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Models
{
    public class UserFriend
    {
        public string User1Id{ get; set; }
        public User User1 { get; set; }
        public string FriendId{ get; set; }
        public User Friend { get; set; }
    }
}
