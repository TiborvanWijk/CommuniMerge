using Communimerge.Api.CustomAttribute;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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


            return Ok();
        }

    }
}
