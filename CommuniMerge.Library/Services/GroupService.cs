using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories;
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
        private readonly IFileUploadRepository fileUploadRepository;

        public GroupService(IGroupRepository groupRepository, ICustomLogger logger, IUserRepository userRepository, IFileUploadRepository fileUploadRepository)
        {
            this.groupRepository = groupRepository;
            this.logger = logger;
            this.userRepository = userRepository;
            this.fileUploadRepository = fileUploadRepository;
        }
        public async Task<CreateGroupResult> CreateGroup(string currentlyLoggedInUserId, GroupCreateDto groupCreateDto)
        {
            try
            {

                if (string.IsNullOrEmpty(groupCreateDto.GroupName) || groupCreateDto.GroupName.Length >= 40)
                {
                    return new CreateGroupResult { Error = CreateGroupError.InvalidGroupName };
                }

                ICollection<User> users = new List<User>();

                users.Add(await userRepository.GetUserByIdAsync(currentlyLoggedInUserId));

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
                    OwnerId = currentlyLoggedInUserId,
                    Description = groupCreateDto.Description,
                };


                bool fileIsEmpty = groupCreateDto.Image == null || groupCreateDto.Image.Length == 0;
                if (!fileIsEmpty)
                {

                    if (!await fileUploadRepository.IsValidFileType(groupCreateDto.Image))
                    {
                        return new CreateGroupResult { Error = CreateGroupError.InvalidFileType };
                    }

                    FileType fileType = await fileUploadRepository.GetFileType(groupCreateDto.Image);

                    if(fileType != FileType.Image)
                    {
                        return new CreateGroupResult { Error = CreateGroupError.InvalidFileType };
                    }


                    var fileName = await fileUploadRepository.UploadFile(groupCreateDto.Image, fileType);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        return new CreateGroupResult { Error = CreateGroupError.FileUpLoadFailed };
                    }
                    group.ProfilePath = Path.Combine("/img/", fileName);
                }

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
                return new CreateGroupResult { Group = group, Error = CreateGroupError.None };
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

        public async Task<ICollection<User>?> GetAllUsersOfGroupById(int groupId)
        {
            try
            {
                ICollection<User> users = await groupRepository.GetAllUsersOfGroupById(groupId);

                return users;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(GetAllUsersOfGroupById));
                return null;
            }
        }

        public async Task<Group?> GetGroupById(int groupId)
        {
            try
            {
                var group = await groupRepository.getGroupById(groupId);

                return group;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(GetGroupById));
                return null;
            }
        }

        public async Task<ICollection<Group>> GetSharedGroups(string userId, string friendId)
        {
            try
            {
                ICollection<Group> sharedGroups = new List<Group>();
                var groups = await groupRepository.GetAllGroupsByUserIdAsync(userId);
                
                
                foreach (var group in groups)
                {
                    bool userIsInGroup = group.UserGroupsLinks.Any(x => x.UserId == userId);
                    bool friendIsInGroup = group.UserGroupsLinks.Any(x => x.UserId == friendId);

                    if(userIsInGroup && friendIsInGroup)
                    {
                        sharedGroups.Add(group);
                    }
                }

                return sharedGroups;
            }catch(Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(IsUserGroupMember));
                return null;
            }
        }

        public async Task<bool> IsUserGroupMember(string loggedInUserId, int groupId)
        {
            try
            {
                bool isMember = await groupRepository.IsInGroup(loggedInUserId, groupId);

                return isMember;
            }catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(IsUserGroupMember));
                return false;
            }
        }
    }
}
