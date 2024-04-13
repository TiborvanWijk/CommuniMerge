using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        Task<bool> CreateGroupAsync(Group group);
        Task<bool> UpdateGroupNameAsync(int groupId, string newName);
        Task<bool> UpdateGroupOwnerAsync(int groupId, string newOwnerId);
        Task<bool> DeleteGroupAsync(int groupId);
        Task<bool> SaveAsync();
        Task<bool> CreateUserGroupLink(UserGroupLink link);
        Task<ICollection<Group>> GetAllGroupsByUserIdAsync(string userId);
        Task<Group> getGroupById(int groupId);
        Task<bool> IsInGroup(string senderId, int groupId);
        Task<ICollection<User>> GetAllUsersOfGroupById(int groupId);
    }
}
