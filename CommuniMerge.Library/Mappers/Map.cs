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
        public static UserDto ToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                ProfilePath = user.ProfilePath,
            };
        }

        public static FriendRequestDto ToFriendRequestDto(FriendRequest friendRequest)
        {
            return new FriendRequestDto
            {
                Sender = ToUserDto(friendRequest.Sender)
            };
        }

        public static GroupDto ToGroupDto(Group group)
        {
            return new GroupDto
            {
                Id = group.Id,
                Name = group.Name,
                Description = group.Description,
                ProfilePath = group.ProfilePath
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
                FilePath = message.FilePath,
                FileType = message.FileType,
            };
        }
    }
}
