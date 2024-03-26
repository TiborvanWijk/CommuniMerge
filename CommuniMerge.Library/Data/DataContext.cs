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
        public DbSet<PrivateConversationLink> PrivateConversations { get; set; }
        public DbSet<PrivateConversationMessageLink> PrivateConversationMessageLinks { get; set; }
        public DbSet<UserGroupLink> userGroupLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PrivateConversationLink>()
                .HasKey(x => x.Id);

            builder.Entity<PrivateConversationLink>()
                .HasOne(pc => pc.User1)
                .WithMany()
                .HasForeignKey(b => b.User1Id)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PrivateConversationLink>()
                .HasOne(pc => pc.User2)
                .WithMany()
                .HasForeignKey(b => b.User2Id)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<GroupMessageLink>()
                            .HasKey(x => new { x.GroupId, x.MessageId });
            builder.Entity<GroupMessageLink>()
                .HasOne(pc => pc.Group)
                .WithMany(u => u.GroupMessageLinks)
                .HasForeignKey(b => b.GroupId);

            builder.Entity<PrivateConversationMessageLink>()
                .HasKey(x => new { x.PrivateConversationId, x.MessageId });
            builder.Entity<PrivateConversationMessageLink>()
                .HasOne(pcml => pcml.PrivateConversation)
                .WithMany(pc => pc.PrivateConversationMessageLinks)
                .HasForeignKey(pcml => pcml.PrivateConversationId);
            builder.Entity<PrivateConversationMessageLink>()
                .HasOne(pcml => pcml.Message)
                .WithMany()
                .HasForeignKey(pcml => pcml.MessageId);

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

        }
    }
}
