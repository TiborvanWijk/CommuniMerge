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
    public class UserController : Controller
    {
        private readonly IAccountService accountService;
        private readonly IMessageService messageService;

        public UserController(IAccountService accountService, IMessageService messageService)
        {
            this.accountService = accountService;
            this.messageService = messageService;
        }

        [HttpPost("sendFriendRequest/{receiverUsername}")]
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

        [HttpPost("acceptFriendRequest/{username}")]
        public async Task<IActionResult> AcceptFriendRequest([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestingUser = await accountService.GetUserByUsernameAsync(username);
            if (requestingUser == null)
            {
                return NotFound();
            }
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await accountService.AcceptFriendRequest(currentUserId, requestingUser.Id);

            if (result.Error != AcceptFriendRequestError.None)
            {
                return StatusCode(501, "ERROR HANDELING IS NOT IMPLEMENTED");
            }

            return Created();
        }


        [HttpPost("declineFriendRequest/{username}")]
        public async Task<IActionResult> DeclineFriendRequest([FromRoute] string username)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requestingUser = await accountService.GetUserByUsernameAsync(username);
            if (requestingUser == null)
            {
                return NotFound();
            }
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await accountService.DeclineFriendRequest(currentUserId, requestingUser.Id);

            if (result.Error != DeclineFriendRequestError.None )
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

            var friendMessages = await Task.WhenAll(friends.Select(async x =>
            {
                var latestMessage = await messageService.GetLatestMessage(loggedInUserId, x.Id);
                return new { User = x, LatestMessage = latestMessage };
            }));

            var orderedFriends = friendMessages
                .OrderByDescending(x =>
                {
                    if(x.LatestMessage == null)
                    {
                        return DateTime.MinValue;
                    }
                    return x.LatestMessage.TimeStamp;
                })
                .ToList();
            List<FriendDisplayDto> friendsWithMessageDto = orderedFriends.Select(x => new FriendDisplayDto
            {
                Username = x.User.UserName,
                LatestMessage = x.LatestMessage == null ? null : Map.ToMessageDisplayDto(x.LatestMessage)
            }).ToList();

            return Ok(friendsWithMessageDto);
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
