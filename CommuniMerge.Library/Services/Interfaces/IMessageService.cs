using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.ResultObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Message> GetLatestMessage(string loggedInUserId, string id);
        Task<MessageCreateResult> CreatePersonalMessage(string senderId, PersonalMessageCreateDto messageCreateDto);
        Task<ICollection<Message>> getPrivateMessages(string userId, string otherUserId);
        Task<Message> GetLatestGroupMessage(int groupId);
        Task<MessageCreateResult> CreateGroupMessage(string? loggedInUserId, GroupMessageCreateDto messageCreateDto);
    }
}
