using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Models;
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
        public DateTime TimeStamp { get; set; }
        public string? FilePath { get; set; }
        public FileType? FileType { get; set; }
        public UserDto sender { get; set; }
    }
}
