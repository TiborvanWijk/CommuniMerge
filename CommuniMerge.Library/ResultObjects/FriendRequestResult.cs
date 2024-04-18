using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.ResultObjects
{
    public class FriendRequestResult
    {
        public FriendRequest FriendRequest { get; set; }
        public FriendRequestError Error { get; set; }
    }
}
