using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.ResultObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services.Interfaces
{
    public interface IAccountService
    {
        Task<RegistrationResult> RegisterAsync(RegisterModel registerModel);
        Task<bool> Login(string username, string password);
        Task<LoginResult> LoginAsync(LoginModel loginModel);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByIdAsync(string userId);
        Task<FriendRequestResult> SendFriendRequest(string senderId, string receiverId);
        Task<AcceptFriendRequestResult> AcceptFriendRequest(string? currentUserId, string id);
        Task<ICollection<User>> GetAllFriends(string userId);
        Task<ICollection<FriendRequest>> GetAllFriendRequests(string loggedInUserId);
        Task<DeclineFriendRequestResult> DeclineFriendRequest(string receiverId, string senderId);
        Task<UpdateUserProfileResult> UpdateUserProfile(string userId, UserUpdateDto userUpdateDto);
    }
}
