﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Enums
{
    public enum MessageCreateError
    {
        None,
        UserNotFound,
        CreateMessageFailed,
        MessageIsNullOrEmpty,
        UnknownError,
        GroupNotFound,
        UnAuthorized,
        InvalidFileType,
        FileUploadFailed
    }
}
