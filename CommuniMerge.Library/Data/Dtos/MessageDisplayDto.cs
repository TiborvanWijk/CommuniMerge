﻿using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class MessageDisplayDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string TimeStamp { get; set; }
        public string SenderUsername { get; set; }
    }
}
