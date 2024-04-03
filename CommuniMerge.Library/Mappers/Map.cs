using CommuniMerge.Library.Data.Dtos;
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
        public static FriendDto ToFriendDto(User user)
        {
            return new FriendDto
            {
                Username = user.UserName
            };
        }

        public static LoginModel ToLoginModelFromRegisterModel(RegisterModel registerModel)
        {
            return new LoginModel { Username = registerModel.Username, Password = registerModel.Password };
        }

        public static MessageDisplayDto ToMessageDisplayDto(Message message)
        {
            return new MessageDisplayDto
            {
                Content = message.Content,
                Id = message.Id,
                SenderUsername = message.SenderUser.UserName,
                TimeStamp = message.TimeStamp.ToShortDateString(),
            };
        }
    }
}
