﻿using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageRepository;
        private readonly ILogger<MessageService> logger;
        private readonly IUserRepository userRepository;

        public MessageService(IMessageRepository messageRepository, ILogger<MessageService> logger, IUserRepository userRepository)
        {
            this.messageRepository = messageRepository;
            this.logger = logger;
            this.userRepository = userRepository;
        }

        public async Task<MessageCreateResult> CreatePersonalMessage(string senderId, PersonalMessageCreateDto messageCreateDto)
        {
            try
            {

                var receiver = await userRepository.GetUserByUsernameAsync(messageCreateDto.ReceiverUsername);
                if(receiver == null)
                {
                    return new MessageCreateResult { Error = MessageCreateError.UserNotFound };
                }

                if(string.IsNullOrEmpty(messageCreateDto.Content))
                {
                    return new MessageCreateResult { Error = MessageCreateError.MessageIsNullOrEmpty };
                }

                var message = new Message
                {
                    SenderId = senderId,
                    SenderUser = await userRepository.GetUserByIdAsync(senderId),
                    ReceiverId = receiver.Id,
                    Receiver = receiver,
                    Content = messageCreateDto.Content,
                    TimeStamp = DateTime.UtcNow,
                };

                if(await messageRepository.CreateMessageAsync(message))
                {
                    return new MessageCreateResult { Error = MessageCreateError.CreateMessageFailed };
                }

                return new MessageCreateResult { Error = MessageCreateError.None };
            }
            catch (Exception ex)
            {
                logger.LogError("TEMP", ex);
                return new MessageCreateResult { Error = MessageCreateError.UnknownError };
            }
        }

        public async Task<ICollection<Message>> getPrivateMessages(string userId, string otherUserId)
        {
            return await messageRepository.GetAllMessagesOfConversationAsync(userId, otherUserId);
        }
    }
}
