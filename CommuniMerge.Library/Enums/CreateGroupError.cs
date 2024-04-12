using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Enums
{
    public enum CreateGroupError
    {
        None,
        UserNotFound,
        Unknown,
        InvalidGroupName,
        FailedCreatingGroup,
        FailedCreatingUserLink,
        DuplicateUserAddition,
        UsersNotFriends
    }
}
