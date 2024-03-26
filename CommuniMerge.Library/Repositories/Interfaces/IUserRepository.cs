﻿using CommuniMerge.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommuniMerge.Library.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByIdAsync(string userId);
        Task<bool> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(string userId);
        Task<bool> DeleteUserByIdAsync(string userId);
        Task<bool> SaveAsync();
        Task<bool> ExistsByEmailAsync(string email);
    }
}
