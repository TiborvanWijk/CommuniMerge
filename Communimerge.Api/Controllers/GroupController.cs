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

            if(result.Error != CreateGroupError.None)
            {
                return StatusCode(501, "THIS IS TEMPORARLY NOT IMPLEMENTED");
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

            List<GroupDisplayDto> groupsWithMessageDto = orderedGroups.Select(x => new GroupDisplayDto
            {
                Id = x.Group.Id,
                Name = x.Group.Name,
                LatestMessage = x.LatestGroupMessage == null ? null : Map.ToMessageDisplayDto(x.LatestGroupMessage)
            }).ToList();

            return Ok(groupsWithMessageDto);

        }
    }
}
