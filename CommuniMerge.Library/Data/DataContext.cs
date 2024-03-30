using CommuniMerge.Library.Models;
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
        public DbSet<GroupMessageLink> GroupMessageLinks { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<UserGroupLink> userGroupLinks { get; set; }
        public DbSet<UserFriend> FriendsLink { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);



            builder.Entity<GroupMessageLink>()
                            .HasKey(x => new { x.GroupId, x.MessageId });
            builder.Entity<GroupMessageLink>()
                .HasOne(pc => pc.Group)
                .WithMany(u => u.GroupMessageLinks)
                .HasForeignKey(b => b.GroupId);

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
                
        }
    }
}
