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
    public class GroupRepository : IGroupRepository
    {
        private readonly DataContext dataContext;

        public GroupRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }
        public async Task<bool> CreateGroupAsync(Group group)
        {
            await dataContext.groups.AddAsync(group);
            return await SaveAsync();
        }

        public async Task<bool> CreateUserGroupLink(UserGroupLink link)
        {
            await dataContext.userGroupLinks.AddAsync(link);
            return await SaveAsync();
        }

        public Task<bool> DeleteGroupAsync(int groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<Group>> GetAllGroupsByUserIdAsync(string userId)
        {
            return await dataContext.groups.Include(x => x.UserGroupsLinks).Where(x => x.UserGroupsLinks.Any(x => x.UserId == userId)).ToListAsync();
        }

        public async Task<Group> getGroupById(int groupId)
        {
            return await dataContext.groups.FirstAsync(x => x.Id == groupId);
        }

        public async Task<bool> IsInGroup(string userId, int groupId)
        {
            return dataContext.groups.Include(x => x.UserGroupsLinks).First(x => x.Id == groupId).UserGroupsLinks.Any(x => x.UserId == userId);
        }

        public async Task<bool> SaveAsync()
        {
            return await dataContext.SaveChangesAsync() > 0;
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
