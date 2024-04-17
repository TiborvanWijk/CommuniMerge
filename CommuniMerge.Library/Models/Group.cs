namespace CommuniMerge.Library.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public string ProfilePath { get; set; } = "/img/doggy.jpg");
        public string Description { get; set; }
        public ICollection<UserGroupLink> UserGroupsLinks { get; set; }
    }
}
