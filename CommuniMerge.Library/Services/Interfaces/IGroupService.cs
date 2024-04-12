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
    public interface IGroupService
    {
        Task<CreateGroupResult> CreateGroup(string currentlyLoggedInUserId, GroupCreateDto groupCreateDto);
        Task<ICollection<Group>> GetAllGroups(string userId);
    }
}
