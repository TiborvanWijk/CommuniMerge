using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class PersonalMessageCreateDto
    {
        public string ReceiverUsername { get; set; }
        public string Content { get; set; }
        public IFormFile? File { get; set; }
    }
}
