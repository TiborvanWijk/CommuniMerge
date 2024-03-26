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

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public async Task<RegistrationResult> Register(RegisterModel registerModel)
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
            if(!await IsValidPassword(registerModel.Password))
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
    }
}
