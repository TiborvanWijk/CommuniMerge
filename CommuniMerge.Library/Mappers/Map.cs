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
                Id = user.Id,
                Username = user.UserName
            };
        }

        public static FriendRequestDto ToFriendRequestDto(FriendRequest friendRequest)
        {
            return new FriendRequestDto
            {
                SenderUsername = friendRequest.Sender.UserName
            };
        }

        public static GroupDto ToGroupDto(Group group)
        {
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
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
                TimeStamp = message.TimeStamp,
            };
        }
    }
}
