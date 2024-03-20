namespace CommuniMerge.Library.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserGroupLink> UserGroupsLinks { get; set; }
        public ICollection<GroupMessageLink> GroupMessageLinks { get; set; }
        public string OwnerId { get; set; }
    }
}
