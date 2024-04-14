using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository messageRepository;
        private readonly ICustomLogger logger;
        private readonly IUserRepository userRepository;
        private readonly IGroupRepository groupRepository;

        public MessageService(IMessageRepository messageRepository, ICustomLogger logger, IUserRepository userRepository, IGroupRepository groupRepository)
        {
            this.messageRepository = messageRepository;
            this.logger = logger;
            this.userRepository = userRepository;
            this.groupRepository = groupRepository;
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
                logger.LogError(ex.Message, GetType().Name, nameof(CreatePersonalMessage));
                return new MessageCreateResult { Error = MessageCreateError.UnknownError };
            }
        }

        public async Task<ICollection<Message>> getPrivateMessages(string userId, string otherUserId)
        {
            return await messageRepository.GetAllMessagesOfConversationAsync(userId, otherUserId);
        }

        public async Task<Message> GetLatestMessage(string loggedInUserId, string id)
        {
            try
            {
                var allMessages = await messageRepository.GetAllMessagesOfConversationAsync(loggedInUserId, id);
                var latestMessage = allMessages.OrderByDescending(x => x.TimeStamp).FirstOrDefault();

                return latestMessage;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(GetLatestMessage));
                return null;
            }
        }

        public async Task<Message> GetLatestGroupMessage(int groupId)
        {
            try
            {

                var allMessages = messageRepository.GetAllMessagesOfGroup(groupId);
                var latestMessage = allMessages.OrderByDescending(x => x.TimeStamp).FirstOrDefault();

                return latestMessage;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(GetLatestGroupMessage));
                return null;
            }
        }

        public async Task<MessageCreateResult> CreateGroupMessage(string senderId, GroupMessageCreateDto messageCreateDto)
        {
            try
            {

                Group group = await groupRepository.getGroupById(messageCreateDto.GroupId);
                if (group == null)
                {
                    return new MessageCreateResult { Error = MessageCreateError.GroupNotFound };
                }
                if(!await groupRepository.IsInGroup(senderId, group.Id))
                {
                    return new MessageCreateResult { Error = MessageCreateError.UnAuthorized };
                }

                if (string.IsNullOrEmpty(messageCreateDto.Content))
                {
                    return new MessageCreateResult { Error = MessageCreateError.MessageIsNullOrEmpty };
                }

                var message = new Message
                {
                    SenderId = senderId,
                    SenderUser = await userRepository.GetUserByIdAsync(senderId),
                    Content = messageCreateDto.Content,
                    TimeStamp = DateTime.UtcNow,
                    Group = group,
                    GroupId = group.Id
                };

                if (await messageRepository.CreateMessageAsync(message))
                {
                    return new MessageCreateResult { Error = MessageCreateError.CreateMessageFailed };
                }

                return new MessageCreateResult { Error = MessageCreateError.None };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(CreateGroupMessage));
                return new MessageCreateResult { Error = MessageCreateError.UnknownError };
            }
        }

        public async Task<ICollection<Message>>? GetGroupMessages(int groupId)
        {
            try
            {
                ICollection<Message> messages = messageRepository.GetAllMessagesOfGroup(groupId);
                return messages;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(CreateGroupMessage));
                return null;
            }
        }
    }
}
