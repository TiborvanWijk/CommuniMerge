using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories.Interfaces
{
    public interface IMessageRepository
    {
        Task<bool> ExistsByIdAsync(int id);
        Task<bool> CreateMessageAsync(Message message);
        Task<bool> DeleteMessageByIdAsync(int id);
        Task<bool> UpdateMessageAsync(Message message);
        Task<ICollection<Message>> GetAllMessagesOfConversationAsync(string currentUser, string otherUser);
        Task<ICollection<Message>> GetAllMessagesOfGroupAsync(int groupId);
        Task<bool> SaveAsync();
    }
}
