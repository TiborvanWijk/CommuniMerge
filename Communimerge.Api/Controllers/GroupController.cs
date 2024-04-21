using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Mappers;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Communimerge.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class GroupController : Controller
    {
        private readonly IGroupService groupService;
        private readonly IAccountService accountService;
        private readonly IMessageService messageService;

        public GroupController(IGroupService groupService, IAccountService accountService, IMessageService messageService)
        {
            this.groupService = groupService;
            this.accountService = accountService;
            this.messageService = messageService;
        }

        [HttpPost("createGroup")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateGroup(GroupCreateDto groupCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentlyLoggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await groupService.CreateGroup(currentlyLoggedInUserId, groupCreateDto);


            switch (result.Error)
            {
                case CreateGroupError.None:
                    break;
                case CreateGroupError.UserNotFound:
                    return NotFound("User(s) not found");
                case CreateGroupError.InvalidGroupName:
                    return BadRequest("Group name must be atleast 1 character long and shorter than 40.");
                case CreateGroupError.DuplicateUserAddition:
                    return BadRequest("Cannot add user twice.");
                case CreateGroupError.UsersNotFriends:
                    return BadRequest("User is not a friend.");
                case CreateGroupError.FailedCreatingGroup:
                    return StatusCode(500, "Something went wrong while creating group.");
                case CreateGroupError.FailedCreatingUserLink:
                    return StatusCode(500, "Something went wrong while adding user(s) to group.");
                case CreateGroupError.FileUpLoadFailed:
                    return StatusCode(500, "Something went wrong while adding group photo.");
                case CreateGroupError.InvalidFileType:
                    return BadRequest("Unsupported file type.");
                case CreateGroupError.Unknown:
                    return StatusCode(500, "Unexpected server error.");
                default:
                    return StatusCode(500, "Unexpected server error.");
            }


            return Ok(result.GroupId);
        }

        [HttpGet("getGroups")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetGroups([FromQuery] bool withLatestMessage)
        {
            var currentlyLoggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ICollection<Group> groups = await groupService.GetAllGroups(currentlyLoggedInUserId);

            if(!withLatestMessage)
            {
                ICollection<GroupDto> groupDtos = groups.Select(Map.ToGroupDto).ToList();
                return Ok(groupDtos);
            }

            var groupMessages = await Task.WhenAll(groups.Select(async group =>
            {
                var latestGroupMessage = await messageService.GetLatestGroupMessage(group.Id);
                return new { Group = group, LatestGroupMessage = latestGroupMessage };
            }));

            var orderedGroups = groupMessages
                .OrderByDescending(x => x.LatestGroupMessage?.TimeStamp ?? DateTime.MinValue)
                .ToList();

            List<GroupDto> groupsWithMessageDto = orderedGroups.Select(x => new GroupDto
            {
                Id = x.Group.Id,
                Name = x.Group.Name,
                Description = x.Group.Description,
                ProfilePath = x.Group.ProfilePath,
                LatestMessage = x.LatestGroupMessage == null ? null : Map.ToMessageDisplayDto(x.LatestGroupMessage)
            }).ToList();

            return Ok(groupsWithMessageDto);
        }

        [HttpGet("getMembers/{groupId:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllMembersOfGroup(int groupId)
        {

            var group = await groupService.GetGroupById(groupId);

            if(group == null)
            {
                return NotFound();
            }
            var currentlyLoggedInUser = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var isMember = await groupService.IsUserGroupMember(currentlyLoggedInUser, groupId);

            if (!isMember)
            {
                return Forbid();
            }

            ICollection<User> users = await groupService.GetAllUsersOfGroupById(groupId);

            ICollection<UserDto> friendDtos = users.Select(Map.ToUserDto).ToList();
            return Ok(friendDtos);
        }
    }
}
