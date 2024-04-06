using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository userRepository;
        private readonly ILogger<AccountService> logger;
        private readonly PasswordHasher<User> passwordHasher;

        public AccountService(IUserRepository userRepository, ILogger<AccountService> logger)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.passwordHasher = new PasswordHasher<User>();
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterModel registerModel)
        {
            var errorResult = await ValidateRegistrationAndGetError(registerModel);
            if (errorResult != RegistrationError.None)
            {
                return new RegistrationResult { Error = errorResult };
            }

            try
            {
                string normalizedEmail = registerModel.Email.ToUpper().Trim();
                string normalizedUsername = registerModel.Username.ToUpper().Trim();
                var user = new User()
                {
                    Email = registerModel.Email,
                    UserName = registerModel.Username,
                    NormalizedEmail = normalizedEmail,
                    NormalizedUserName = normalizedUsername,
                };

                var hashedPassword = passwordHasher.HashPassword(user, registerModel.Password);
                user.PasswordHash = hashedPassword;

                if (!await userRepository.CreateUserAsync(user))
                {
                    return new RegistrationResult { Error = RegistrationError.CreateUserFailed };
                }

                return new RegistrationResult { Error = RegistrationError.None };
            }
            catch (Exception ex)
            {
                logger.LogError($"[{DateTime.UtcNow}] AccountService.Register: {ex}");
                return new RegistrationResult { Error = RegistrationError.UnknownError };
            }
        }

        private async Task<RegistrationError> ValidateRegistrationAndGetError(RegisterModel registerModel)
        {

            if (await userRepository.ExistsByEmailAsync(registerModel.Email))
            {
                return RegistrationError.EmailExists;
            }
            if (!await IsValidEmail(registerModel.Email))
            {
                return RegistrationError.InvalidEmailFormat;
            }
            if (!await IsValidPassword(registerModel.Password))
            {
                return RegistrationError.WeakPassword;
            }
            return RegistrationError.None;
        }
        private async Task<bool> IsValidPassword(string password)
        {
            return true;
        }
        private async Task<bool> IsValidEmail(string email)
        {
            return true;
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            var user = await userRepository.GetUserByUsernameAsync(loginModel.Username);
            var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return new LoginResult { Error = LoginError.InvalidCombination };
            }

            return new LoginResult { Error = LoginError.None };
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await userRepository.GetUserByUsernameAsync(username);
        }

        public async Task<User?> GetUserByIdAsync(string? userId)
        {
            return await userRepository.GetUserByIdAsync(userId);
        }

        public async Task<FriendRequestResult> SendFriendRequest(string senderId, string receiverId)
        {
            try
            {

                if (await userRepository.FriendRequestExists(senderId, receiverId))
                {
                    return new FriendRequestResult { Error = FriendRequestError.RequestExists };
                }
                if (await userRepository.AreFriends(senderId, receiverId))
                {
                    return new FriendRequestResult { Error = FriendRequestError.AlreadyFriends };
                }

                var friendRequest = new FriendRequest
                {
                    ReceiverId = receiverId,
                    Receiver = await userRepository.GetUserByIdAsync(receiverId),
                    SenderId = senderId,
                    Sender = await userRepository.GetUserByIdAsync(senderId)
                };

                if (!await userRepository.CreateFriendRequest(friendRequest))
                {
                    return new FriendRequestResult { Error = FriendRequestError.CreateRequestFailed };
                }

                return new FriendRequestResult { Error = FriendRequestError.None };

            }
            catch (Exception ex)
            {
                logger.LogError("Temp", ex);
                return new FriendRequestResult { Error = FriendRequestError.UnknownError };
            }
        }

        public async Task<AcceptFriendRequestResult> AcceptFriendRequest(string currentUserId, string requestingUserId)
        {
            try
            {

                if(!await userRepository.FriendRequestExists(currentUserId, requestingUserId))
                {
                    return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.RequestNotFound };
                }
                if(await userRepository.AreFriends(currentUserId, requestingUserId))
                {
                    return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.AlreadyFriends };
                }
                var userFriend = new UserFriend
                {
                    User1Id = currentUserId,
                    User1 = await userRepository.GetUserByIdAsync(currentUserId),
                    FriendId = requestingUserId,
                    Friend = await userRepository.GetUserByIdAsync(requestingUserId),
                };

                if (!await userRepository.AddFriend(userFriend))
                {
                    return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.AcceptRequestFailed };
                }
                if(!await userRepository.DeleteRequest(currentUserId, requestingUserId))
                {
                    return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.DeleteRequestFailed };
                }

                return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.None };
            }
            catch (Exception ex)
            {
                logger.LogError("Temp", ex);
                return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.UnknownError };
            }
        }

        public async Task<ICollection<User>> GetAllFriends(string userId)
        {
            try
            {
                return await userRepository.getAllFriendsById(userId);
            }catch(Exception ex)
            {
                logger.LogError("TEMP", ex);
                return null;
            }
        }



        public async Task<ICollection<FriendRequest>> GetAllFriendRequests(string userId)
        {
            try
            {
                return await userRepository.GetAllFriendRequestsById(userId);
            }catch(Exception ex)
            {
                logger.LogError("TEMP", ex);
                return null;
            }
        }

        public async Task<DeclineFriendRequestResult>  DeclineFriendRequest(string receiverId, string senderId)
        {
            try
            {

                if (!await userRepository.FriendRequestExists(receiverId, senderId))
                {
                    return new DeclineFriendRequestResult { Error = DeclineFriendRequestError.RequestNotFound };
                }

                if (!await userRepository.DeleteRequest(receiverId, senderId))
                {
                    return new DeclineFriendRequestResult { Error = DeclineFriendRequestError.DeleteRequestFailed };
                }

                return new DeclineFriendRequestResult { Error = DeclineFriendRequestError.None };
            }
            catch (Exception ex) 
            {
                logger.LogError("TEMP", ex);
                return new DeclineFriendRequestResult { Error = DeclineFriendRequestError.UnknownError };
            }
        }
    }
}
