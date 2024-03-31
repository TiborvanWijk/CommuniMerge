using Communimerge.Api.CustomAttribute;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Communimerge.Api.Controllers
{
    [ApiController]
    [Route("/api/messages")]
    [Authorize]
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
        public IActionResult GetPrivateMessages([FromRoute] string username) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok();
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
