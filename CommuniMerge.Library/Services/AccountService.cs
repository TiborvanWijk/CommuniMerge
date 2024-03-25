using CommuniMerge.Library.Repositories.Interfaces;
using CommuniMerge.Library.Services.Interfaces;
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

        public AccountService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public Task<bool> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Register(string username, string email, string password)
        {
            throw new NotImplementedException();
        }
        //private Task IsValidLogin(string username, string password)
        //{

        //}
    }
}
