using Communimerge.Api.CustomAttribute;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Mappers;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Communimerge.Api.Controllers
{
    [ApiController]
    [Route("/api/messages")]
    [Authorize]
    [EnableCors]
    public class MessageController : Controller
    {
        private readonly IMessageService messageService;
        private readonly IAccountService accountService;
        private readonly IGroupService groupService;

        public MessageController(IMessageService messageService, IAccountService accountService, IGroupService groupService)
        {
            this.messageService = messageService;
            this.accountService = accountService;
            this.groupService = groupService;
        }

        
        [HttpGet("/get/{username}")]
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

        [HttpGet("/getGroup{groupId:int}")]
        public async Task<IActionResult> GetGroupMessages([FromRoute] int groupId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            return Ok();
        }


        [HttpPost("/createPersonalMessage")]
        public async Task<IActionResult> CreatePersonalMessage([FromBody] PersonalMessageCreateDto messageCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await messageService.CreatePersonalMessage(loggedInUserId, messageCreateDto);

            if(result.Error != MessageCreateError.None)
            {
                return StatusCode(501, "THIS IS TEMPORARLY NOT IMPLEMENTED");
            }


            return Created();
        }

        [HttpPost("/CreateGroupMessage")]
        public async Task<IActionResult> CreateGroupMessage([FromBody] GroupMessageCreateDto messageCreateDto)
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;






            return Created();
        }

    }
}
