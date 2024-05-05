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

        public async Task<bool> AddFriend(UserFriend userFriend)
        {
            await dataContext.FriendsLink.AddAsync(userFriend);
            return await SaveAsync();
        }

        public async Task<bool> AreFriends(string user1Id, string user2Id)
        {
            return dataContext.FriendsLink.Any(x => 
            (x.User1Id == user1Id && x.FriendId == user2Id)
            || (x.FriendId == user1Id && x.User1Id == user2Id));
        }

        public async Task<bool> CreateFriendRequest(FriendRequest friendRequest)
        {
            await dataContext.FriendRequests.AddAsync(friendRequest);
            return await SaveAsync();
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            await dataContext.Users.AddAsync(user);
            return await SaveAsync();
        }

        public async Task<bool> DeleteRequest(string currentUserId, string requestingUserId)
        {
            dataContext.FriendRequests.Remove(await dataContext.FriendRequests.FirstAsync(x => 
                (x.SenderId == currentUserId && x.ReceiverId == requestingUserId)
                || (x.SenderId == requestingUserId && x.ReceiverId == currentUserId)));
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

        public async Task<bool> FriendRequestExists(string user1Id, string user2Id)
        {
            return await dataContext.FriendRequests.AnyAsync(x => 
            x.SenderId == user1Id && x.ReceiverId == user2Id 
            || x.ReceiverId == user1Id && x.SenderId == user2Id);
        }

        public async Task<ICollection<FriendRequest>> GetAllFriendRequestsById(string userId)
        {
            return await dataContext.FriendRequests.Include(x => x.Sender).Where(x => x.ReceiverId == userId).ToListAsync();
        }

        public async Task<ICollection<User>> getAllFriendsById(string userId)
        {
            return await dataContext.FriendsLink.Where(x => 
            x.User1Id == userId || x.FriendId == userId)
                .Select(x => x.User1Id == userId ? x.Friend : x.User1)
                .ToListAsync();
        }



        public async Task<User> GetUserByIdAsync(string userId)
        {
            return await dataContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }


        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await dataContext.Users.FirstOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<bool> SaveAsync()
        {
            var saved = dataContext.SaveChangesAsync();
            return await saved > 0;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            dataContext.Users.Update(user);
            return await SaveAsync();
        }
    }
}
