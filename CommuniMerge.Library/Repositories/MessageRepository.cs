using CommuniMerge.Library.Data;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext dataContext;

        public MessageRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<bool> CreateMessageAsync(Message message)
        {
            await dataContext.Messages.AddAsync(message);
            return await SaveAsync();
        }

        public async Task<bool> DeleteMessageByIdAsync(int id)
        {
            dataContext.Messages.Remove(dataContext.Messages.First(x => x.Id == id));
            return await SaveAsync();
        }

        public async Task<bool> ExistsByIdAsync(int id)
        {
            return dataContext.Messages.Any(x => x.Id == id);
        }

        public async Task<ICollection<Message>> GetAllMessagesOfConversationAsync(string currentUserId, string otherUserId)
        {
            return dataContext.Messages.Where(x =>
            (x.SenderId == currentUserId && x.ReceiverId == otherUserId)
            || (x.SenderId == otherUserId && x.ReceiverId == currentUserId))
                .OrderBy(x => x.TimeStamp).ToList();
        }

        public Task<ICollection<Message>> GetAllMessagesOfGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> SaveAsync()
        {
            bool saved = await dataContext.SaveChangesAsync() > 1;
            return saved;
        }

        public async Task<bool> UpdateMessageAsync(Message message)
        {
            dataContext.Messages.Update(message);
            return await SaveAsync();
        }
    }
}
