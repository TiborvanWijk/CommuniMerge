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

        [HttpPost("/acceptFriendRequest/{username}")]
        public async Task<IActionResult> AcceptFriendRequest([FromRoute] string username)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var requestingUser = await accountService.GetUserByUsernameAsync(username);
            if(requestingUser == null)
            {
                return NotFound();
            }
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await accountService.AcceptFriendRequest(currentUserId, requestingUser.Id);

            if(result.Error != AcceptFriendRequestError.None)
            {
                return StatusCode(501, "ERROR HANDELING IS NOT IMPLEMENTED");
            }

            return Created();
        }
        [HttpGet("friends")]
        public async Task<IActionResult> GetAllFriends([FromQuery] bool withLatestMessage)
        {

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ICollection<User> friends = await accountService.GetAllFriends(loggedInUserId);


            if(!withLatestMessage)
            {
                var friendsDto = friends.Select(Map.ToFriendDto);
                return Ok(friendsDto);
            }


            var friendsWithMessageDto = await Task.WhenAll(friends.Select(async x =>
            {
                var latestMessage = await accountService.GetLatestMessage(loggedInUserId, x.Id);
                FriendDisplayDto friendDisplayDto = new FriendDisplayDto()
                {
                    Username = x.UserName,
                    LatestMessage = latestMessage == null ? null : Map.ToMessageDisplayDto(latestMessage)
                };
                return friendDisplayDto;
            }));


            return Ok(friendsWithMessageDto.ToList());
        }

        [HttpGet("friendRequests")]
        public async Task<IActionResult> GetAllFriendRequests()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ICollection<FriendRequest> friendRequests = await accountService.GetAllFriendRequests(loggedInUserId);

            ICollection<FriendRequestDto> friendRequestDtos = friendRequests.Select(Map.ToFriendRequestDto).ToList();

            return Ok(friendRequestDtos);
        }

    }
}
