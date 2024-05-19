using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class GroupDto
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public string? ProfilePath { get; set; }
        public MessageDisplayDto LatestMessage { get; set; }
    }
}
