using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Loggers.Interfaces;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository userRepository;
        private readonly ICustomLogger logger;
        private readonly IFileUploadRepository fileUploadRepository;
        private readonly PasswordHasher<User> passwordHasher;

        public AccountService(IUserRepository userRepository, ICustomLogger logger, IFileUploadRepository fileUploadRepository)
        {
            this.userRepository = userRepository;
            this.logger = logger;
            this.fileUploadRepository = fileUploadRepository;
            this.passwordHasher = new PasswordHasher<User>();
        }

        public async Task<RegistrationResult> RegisterAsync(RegisterModel registerModel)
        {
            try
            {
                var errorResult = await ValidateRegistrationAndGetError(registerModel);
                if (errorResult != RegistrationError.None)
                {
                    return new RegistrationResult { Error = errorResult };
                }

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
                logger.LogError(ex.Message, GetType().Name, nameof(RegisterAsync));
                return new RegistrationResult { Error = RegistrationError.UnknownError };
            }
        }

        private async Task<RegistrationError> ValidateRegistrationAndGetError(RegisterModel registerModel)
        {
            if (!await isValidUsername(registerModel.Username))
            {
                return RegistrationError.InvalidUsername;
            }

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

        private async Task<bool> isValidUsername(string username)
        {
            if (await userRepository.GetUserByUsernameAsync(username) != null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(username) || username.Length >= 20)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> IsValidPassword(string password)
        {
            string pattern = @"^(?=.*[!@#$%^&*()-_+=])[A-Za-z0-9!@#$%^&*()-_+=]{8,}$";
            bool isValid = Regex.IsMatch(password, pattern);

            return isValid;
        }
        private async Task<bool> IsValidEmail(string email)
        {
            if (email == null)
            {
                return false;
            }
            return true;
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginResult> LoginAsync(LoginModel loginModel)
        {
            try
            {
                var user = await userRepository.GetUserByUsernameAsync(loginModel.Username);

                if (user == null)
                {
                    return new LoginResult { Error = LoginError.InvalidCombination };
                }

                var result = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginModel.Password);

                if (result == PasswordVerificationResult.Failed)
                {
                    return new LoginResult { Error = LoginError.InvalidCombination };
                }

                return new LoginResult { Error = LoginError.None };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(LoginAsync));
                return new LoginResult { Error = LoginError.UnExpected };
            }
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

                if (senderId.Equals(receiverId))
                {
                    return new FriendRequestResult { Error = FriendRequestError.ToSelf };
                }
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

                return new FriendRequestResult { FriendRequest = friendRequest, Error = FriendRequestError.None };

            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(SendFriendRequest));
                return new FriendRequestResult { Error = FriendRequestError.UnknownError };
            }
        }

        public async Task<AcceptFriendRequestResult> AcceptFriendRequest(string currentUserId, string requestingUserId)
        {
            try
            {

                if (!await userRepository.FriendRequestExists(currentUserId, requestingUserId))
                {
                    return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.RequestNotFound };
                }
                if (await userRepository.AreFriends(currentUserId, requestingUserId))
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
                if (!await userRepository.DeleteRequest(currentUserId, requestingUserId))
                {
                    return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.DeleteRequestFailed };
                }

                return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.None };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(AcceptFriendRequest));
                return new AcceptFriendRequestResult { Error = AcceptFriendRequestError.UnknownError };
            }
        }

        public async Task<ICollection<User>> GetAllFriends(string userId)
        {
            try
            {
                return await userRepository.getAllFriendsById(userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(GetAllFriends));
                return null;
            }
        }



        public async Task<ICollection<FriendRequest>> GetAllFriendRequests(string userId)
        {
            try
            {
                return await userRepository.GetAllFriendRequestsById(userId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(GetAllFriendRequests));
                return null;
            }
        }

        public async Task<DeclineFriendRequestResult> DeclineFriendRequest(string receiverId, string senderId)
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
                logger.LogError(ex.Message, GetType().Name, nameof(DeclineFriendRequest));
                return new DeclineFriendRequestResult { Error = DeclineFriendRequestError.UnknownError };
            }
        }

        public async Task<UpdateUserProfileResult> UpdateUserProfile(string userId, UserUpdateDto userUpdateDto)
        {
            try
            {
                bool allPropertiesAreNull = userUpdateDto.GetType().GetProperties().All(prop =>
                {
                    var value = prop.GetValue(userUpdateDto);
                    return value == null;
                });

                if (allPropertiesAreNull)
                {
                    return new UpdateUserProfileResult { Error = UpdateUserProfileError.AllPropertiesAreNull };
                }

                bool usernameIsValid = !string.IsNullOrEmpty(userUpdateDto.Username) && userUpdateDto.Username.Length <= 20;

                if (!usernameIsValid)
                {
                    return new UpdateUserProfileResult { Error = UpdateUserProfileError.InvalidUsername };
                }

                bool aboutIsToLong = userUpdateDto.About?.Length >= 100;

                if (aboutIsToLong)
                {
                    return new UpdateUserProfileResult { Error = UpdateUserProfileError.AboutIsToLong };
                }

                var userWithInputUsername = await userRepository.GetUserByUsernameAsync(userUpdateDto.Username);

                bool usernameInUse = userWithInputUsername != null && !userWithInputUsername.Id.Equals(userId);
                usernameInUse = usernameInUse ? userWithInputUsername != null : usernameInUse;

                if (usernameInUse)
                {
                    return new UpdateUserProfileResult { Error = UpdateUserProfileError.InvalidUsername };
                }

                var currentUser = await userRepository.GetUserByIdAsync(userId);

                if (userUpdateDto.ProfileImage != null)
                {

                    bool fileIsNotAImage = await fileUploadRepository.GetFileType(userUpdateDto.ProfileImage) != FileType.Image;

                    if (fileIsNotAImage)
                    {
                        return new UpdateUserProfileResult { Error = UpdateUserProfileError.InValidFileType };
                    }

                    string? imagePath = await fileUploadRepository.UploadFile(userUpdateDto.ProfileImage, FileType.Image);
                    bool fileIsUploaded = imagePath != null;

                    if (!fileIsUploaded)
                    {
                        return new UpdateUserProfileResult { Error = UpdateUserProfileError.FailedUploadingImage };
                    }
                    currentUser.ProfilePath = "/img/" + imagePath;
                }

                currentUser.UserName = userUpdateDto.Username == null ? currentUser.UserName : userUpdateDto.Username;
                currentUser.NormalizedUserName = userUpdateDto.Username == null ? currentUser.UserName : userUpdateDto.Username.Trim().ToUpper();
                currentUser.About = userUpdateDto.About == null ? currentUser.About : userUpdateDto.About;

                if (!await userRepository.UpdateUserAsync(currentUser))
                {
                    return new UpdateUserProfileResult { Error = UpdateUserProfileError.FailedUpdatingUserInfo };
                }

                return new UpdateUserProfileResult { Error = UpdateUserProfileError.None };
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, GetType().Name, nameof(DeclineFriendRequest));
                return new UpdateUserProfileResult { Error = UpdateUserProfileError.UnknownError };
            }
        }
    }
}
