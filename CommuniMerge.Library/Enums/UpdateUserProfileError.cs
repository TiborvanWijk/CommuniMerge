using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Enums
{
    public enum UpdateUserProfileError
    {
        None,
        AllPropertiesAreNull,
        InValidFileType,
        InvalidUsername,
        FailedUploadingImage,
        AboutIsToLong,
        FailedUpdatingUserInfo,
        UnknownError,
    }
}
