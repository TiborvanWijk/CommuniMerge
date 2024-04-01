using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class FriendDisplayDto
    {
        public string Username { get; set; }
        public MessageDisplayDto LatestMessage { get; set; }
    }
}
