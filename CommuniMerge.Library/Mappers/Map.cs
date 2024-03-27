using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Mappers
{
    public abstract class Map
    {
        public static LoginModel ToLoginModelFromRegisterModel(RegisterModel registerModel)
        {
            return new LoginModel { Username = registerModel.Username, Password = registerModel.Password };
        }
    }
}
