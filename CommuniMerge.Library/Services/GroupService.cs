using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services.Interfaces;

namespace CommuniMerge.Library.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository groupRepository;
        private readonly ICustomLogger logger;
        private readonly IUserRepository userRepository;

        public GroupService(IGroupRepository groupRepository, ICustomLogger logger, IUserRepository userRepository)
        {
            this.groupRepository = groupRepository;
            this.logger = logger;
            this.userRepository = userRepository;
        }
        public async Task<CreateGroupResult> CreateGroup(string currentlyLoggedInUserId, GroupCreateDto groupCreateDto)
        {
            try
            {

                if (string.IsNullOrEmpty(groupCreateDto.GroupName) || groupCreateDto.GroupName.Length >= 40)
                {
                    return new CreateGroupResult { Error = CreateGroupError.InvalidGroupName };
                }

                ICollection<User> users = new List<User>()
                {
                    await userRepository.GetUserByIdAsync(currentlyLoggedInUserId)
                };

                foreach (var username in groupCreateDto.Usernames)
                {
                    User? user = await userRepository.GetUserByUsernameAsync(username);

                    if (user == null)
                    {
                        return new CreateGroupResult { Error = CreateGroupError.UserNotFound };
                    }
                    else if(!await userRepository.AreFriends(currentlyLoggedInUserId, user.Id))
                    {
                        return new CreateGroupResult { Error = CreateGroupError.UsersNotFriends };
                    }
                    else if (users.Contains(user))
                    {
                        return new CreateGroupResult { Error = CreateGroupError.DuplicateUserAddition };
                    }
                    users.Add(user);
                }

                var group = new Group
                {
                    Name = groupCreateDto.GroupName,
                    OwnerId = currentlyLoggedInUserId
                };
                var groupIsCreated = await groupRepository.CreateGroupAsync(group);

                if (!groupIsCreated)
                {
                    return new CreateGroupResult { Error = CreateGroupError.FailedCreatingGroup };
                }

                ICollection<UserGroupLink> links = new List<UserGroupLink>();

                foreach (var user in users)
                {
                    var link = new UserGroupLink
                    {
                        Group = group,
                        GroupId = group.Id,
                        User = user,
                        UserId = user.Id,
                        JoinDate = DateTime.UtcNow,
                    };
                    links.Add(link);
                }

                foreach (var link in links)
                {
                    var linkIsCreated = await groupRepository.CreateUserGroupLink(link);
                    if (!linkIsCreated)
                    {
                        return new CreateGroupResult { GroupId = group.Id, Error = CreateGroupError.FailedCreatingUserLink };
                    }
                }
                return new CreateGroupResult { Error = CreateGroupError.None };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(CreateGroup));
                return new CreateGroupResult { Error = CreateGroupError.Unknown };
            }
        }

        public async Task<ICollection<Group>> GetAllGroups(string userId)
        {
            try
            {
                return await groupRepository.GetAllGroupsByUserIdAsync(userId);

            }catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(CreateGroup));
                return null;
            }
        }
    }
}
