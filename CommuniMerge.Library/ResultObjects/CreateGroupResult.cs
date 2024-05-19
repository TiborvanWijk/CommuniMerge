using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.ResultObjects
{
    public class CreateGroupResult
    {
        public Group Group { get; set; }
        public int? GroupId { get; set; }
        public CreateGroupError Error { get; set; }
    }
}
