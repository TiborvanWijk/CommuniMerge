using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services.Interfaces
{
    public interface IMessageService
    {
        Task<ICollection<Message>> getPrivateMessages(string userId, string otherUserId);
    }
}
