using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class PersonalMessageCreateDto
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
}
