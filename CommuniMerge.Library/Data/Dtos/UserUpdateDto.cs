using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Data.Dtos
{
    public class UserUpdateDto
    {

        public string? About { get; set; }
        public string? Username { get; set; }
        public IFormFile? ProfileImage { get; set; }

    }
}
