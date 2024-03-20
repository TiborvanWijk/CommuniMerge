namespace CommuniMerge.Models
{
    public class UserGroupLink
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime JoinDate { get; set; }
    }
}
