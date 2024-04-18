using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Enums
{
    public enum FriendRequestError
    {
        None,
        RequestExists,
        AlreadyFriends,
        CreateRequestFailed,
        UnknownError,
        ToSelf
    }
}
