using Communimerge.Api.Hubs;
using Communimerge.Api.Hubs.ClientInterfaces;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Mappers;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Communimerge.Api.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("/api/[controller]")]
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService messageService;
        private readonly IAccountService accountService;
        private readonly IGroupService groupService;
        private readonly IHubContext<ChatHub, IChatClient> chatHub;

        public MessageController(IMessageService messageService, IAccountService accountService, IGroupService groupService, IHubContext<ChatHub, IChatClient> chatHub)
        {
            this.messageService = messageService;
            this.accountService = accountService;
            this.groupService = groupService;
            this.chatHub = chatHub;
        }


        [HttpGet("get/{username}")]
        public async Task<IActionResult> GetPersonalMessages([FromRoute] string username) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await accountService.GetUserByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ICollection<Message> messages = await messageService.getPrivateMessages(loggedInUserId, user.Id);

            var messageDtos = messages.Select(Map.ToMessageDisplayDto);

            return Ok(messageDtos);
        }

        [HttpGet("getGroup/{groupId:int}")]
        public async Task<IActionResult> GetGroupMessages([FromRoute] int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Group group = await groupService.GetGroupById(groupId);

            if (group == null)
            {
                return NotFound();
            }
            bool isMember = await groupService.IsUserGroupMember(loggedInUserId, groupId);
            if(!isMember)
            {
                return Forbid();
            }


            ICollection<Message>? messages = await messageService.GetGroupMessages(groupId);

            if(messages == null)
            {
                return StatusCode(500, "Unexpected error.");
            }

            ICollection<MessageDisplayDto> messageDisplayDtos = messages.Select(Map.ToMessageDisplayDto).ToList();

            return Ok(messageDisplayDtos);
        }


        [HttpPost("createPersonalMessage")]
        public async Task<IActionResult> CreatePersonalMessage([FromForm] PersonalMessageCreateDto messageCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await messageService.CreatePersonalMessage(loggedInUserId, messageCreateDto);


            switch (result.Error)
            {
                case MessageCreateError.None:
                    break;
                case MessageCreateError.UserNotFound:
                    return NotFound("User not found.");
                case MessageCreateError.MessageIsNullOrEmpty:
                    return BadRequest("Message may not be empty.");
                case MessageCreateError.UnAuthorized:
                    return Unauthorized();
                case MessageCreateError.UnknownError:
                    return StatusCode(500, "Unexpected server error.");
                case MessageCreateError.InvalidFileType:
                    return BadRequest("Unsupported file type.");
                case MessageCreateError.FileUploadFailed:
                    return StatusCode(500, "Something went wrong while adding file.");
                default:
                    return StatusCode(500, "Unexpected server error.");
            }

            var receiver = await accountService.GetUserByUsernameAsync(messageCreateDto.ReceiverUsername);

            MessageDisplayDto messageDisplayDto = Map.ToMessageDisplayDto(result.Message);
            UserDto sender = Map.ToUserDto(await accountService.GetUserByIdAsync(loggedInUserId));
            await chatHub.Clients.User(loggedInUserId).ReceiveMessage(receiver.UserName, messageDisplayDto);
            await chatHub.Clients.User(receiver.Id).ReceiveMessage(receiver.UserName, messageDisplayDto);

            return Created();
        }

        [HttpPost("CreateGroupMessage")]
        public async Task<IActionResult> CreateGroupMessage([FromForm] GroupMessageCreateDto messageCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            MessageCreateResult result = await messageService.CreateGroupMessage(loggedInUserId, messageCreateDto);

            switch (result.Error)
            {
                case MessageCreateError.None:
                    break;
                case MessageCreateError.GroupNotFound:
                    return NotFound("Group not found.");
                case MessageCreateError.MessageIsNullOrEmpty:
                    return BadRequest("Message may not be empty.");
                case MessageCreateError.UnAuthorized:
                    return Unauthorized();
                case MessageCreateError.UnknownError:
                    return StatusCode(500, "Unexpected server error.");
                case MessageCreateError.InvalidFileType:
                    return BadRequest("Unsupported file type.");
                case MessageCreateError.FileUploadFailed:
                    return StatusCode(500, "Something went wrong while adding file.");
                default:
                    return StatusCode(500, "Unexpected server error.");
            }

            var receivers = await groupService.GetAllUsersOfGroupById(messageCreateDto.GroupId);

            UserDto sender = Map.ToUserDto(await accountService.GetUserByIdAsync(loggedInUserId));
            MessageDisplayDto messageDisplayDto = Map.ToMessageDisplayDto(result.Message);
            foreach (var user in receivers)
            {
                await chatHub.Clients.User(user.Id).ReceiveGroupMessage(messageCreateDto.GroupId, messageDisplayDto);
            }


            return Created();
        }

    }
}
