using CommuniMerge.Library.Data;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext dataContext;

        public UserRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            await dataContext.Users.AddAsync(user);
            return await SaveAsync();
        }

        public Task<bool> DeleteUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            return await dataContext.Users.AnyAsync(x => x.Email == email);
        }

        public Task<bool> ExistsByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await dataContext.Users.FirstAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAsync()
        {
            var saved = dataContext.SaveChangesAsync();
            return await saved > 0;
        }
    }
}
