using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class GroupCreateDto
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public IFormFile? Image { get; set; }
        public ICollection<string> Usernames { get; set; }
    }
}
