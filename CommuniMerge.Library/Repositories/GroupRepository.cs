using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        public Task<bool> CreateGroupAsync(Group group)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGroupNameAsync(int groupId, string newName)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateGroupOwnerAsync(int groupId, string newOwnerId)
        {
            throw new NotImplementedException();
        }
    }
}
