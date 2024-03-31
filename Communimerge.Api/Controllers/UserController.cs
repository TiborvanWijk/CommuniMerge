using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Communimerge.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IAccountService accountService;

        public UserController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("/sendFriendRequest/{receiverUsername}")]
        public async Task<IActionResult> SendFriendRequest([FromRoute] string receiverUsername)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);
            if(receiver == null)
            {
                return NotFound();
            }
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await accountService.SendFriendRequest(loggedInUserId, receiver.Id);

            if(result.Error != FriendRequestError.None)
            {
                return StatusCode(501, "ERROR HANDELING IS NOT IMPLEMENTED");
            }


            return Created();
        }

    }
}
