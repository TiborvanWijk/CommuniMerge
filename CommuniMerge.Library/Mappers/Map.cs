﻿using CommuniMerge.Library.Data.Dtos;
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

        public static MessageDisplayDto ToMessageDisplayDto(Message message)
        {
            return new MessageDisplayDto
            {
                Content = message.Content,
                Id = message.Id,
                SenderId = message.SenderId,
                TimeStamp = message.TimeStamp,
            };
        }
    }
}