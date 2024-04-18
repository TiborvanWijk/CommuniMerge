using Communimerge.Api.Hubs;
using Communimerge.Api.Hubs.ClientInterfaces;
using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Mappers;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
        private readonly IHubContext<FriendHub, IFriendClient> friendHub;

        public UserController(IAccountService accountService, IMessageService messageService, IHubContext<FriendHub, IFriendClient> friendHub)
        {
            this.accountService = accountService;
            this.messageService = messageService;
            this.friendHub = friendHub;
        }

        [HttpPost("sendFriendRequest/{receiverUsername}")]
        public async Task<IActionResult> SendFriendRequest([FromRoute] string receiverUsername)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var receiver = await accountService.GetUserByUsernameAsync(receiverUsername);
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (receiver == null)
            {
                await friendHub.Clients.User(loggedInUserId).FailSendingFriendRequest("User does not exist");
                return NotFound();
            }

            var result = await accountService.SendFriendRequest(loggedInUserId, receiver.Id);

            if(result.Error != FriendRequestError.None)
            {
                await friendHub.Clients.User(loggedInUserId).FailSendingFriendRequest("Something went wrong");
                return StatusCode(501, "ERROR HANDELING IS NOT IMPLEMENTED");
            }

            FriendRequestDto friendRequestDto = Map.ToFriendRequestDto(result.FriendRequest);

            await friendHub.Clients.User(receiver.Id).ReceiveFriendRequest(friendRequestDto);
            await friendHub.Clients.User(loggedInUserId).SuccesSendingFriendRequest("Succesfully sent friendrequest");

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

            var currentUserDto = Map.ToUserDto(await accountService.GetUserByIdAsync(currentUserId));
            var requestingUserDto = Map.ToUserDto(requestingUser);

            await friendHub.Clients.User(currentUserId).UpdateFriendListing(currentUserDto, requestingUserDto);
            await friendHub.Clients.User(requestingUser.Id).UpdateFriendListing(currentUserDto, requestingUserDto);
            await friendHub.Clients.User(currentUserId).DeleteFriendRequestListing(username);

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

            await friendHub.Clients.User(currentUserId).DeleteFriendRequestListing(username);


            return Created();
        }

        [HttpGet("friends")]
        public async Task<IActionResult> GetAllFriends([FromQuery] bool withLatestMessage)
        {

            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ICollection<User> friends = await accountService.GetAllFriends(loggedInUserId);


            if(!withLatestMessage)
            {
                var friendsDto = friends.Select(Map.ToUserDto);
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
                ProfilePath = x.User.ProfilePath,
                Id = x.User.Id,
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
