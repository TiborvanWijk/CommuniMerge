﻿using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CommuniMerge.Library.Data
{
    public class DataContext : IdentityDbContext<User>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Group> groups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserGroupLink> userGroupLinks { get; set; }
        public DbSet<UserFriend> FriendsLink { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<UserGroupLink>()
                .HasKey(x => new { x.UserId, x.GroupId });
            builder.Entity<UserGroupLink>()
                .HasOne(pc => pc.User)
                .WithMany(u => u.UserGroupsLinks)
                .HasForeignKey(b => b.UserId);
            builder.Entity<UserGroupLink>()
                .HasOne(pc => pc.Group)
                .WithMany(ug => ug.UserGroupsLinks)
                .HasForeignKey(b => b.GroupId);

            builder.Entity<UserFriend>()
                .HasKey(x => new { x.User1Id, x.FriendId });
            builder.Entity<UserFriend>()
                .HasOne(x => x.User1)
                .WithMany(x => x.FriendsLink)
                .HasForeignKey(x => x.User1Id)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserFriend>()
                .HasOne(x => x.Friend)
                .WithMany()
                .HasForeignKey(x => x.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<FriendRequest>()
                .HasKey(x => new { x.SenderId, x.ReceiverId});
            builder.Entity<FriendRequest>()
                .HasOne(x => x.Sender)
                .WithMany(x => x.FriendRequests)
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<FriendRequest>()
                .HasOne(x => x.Receiver)
                .WithMany()
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
