namespace CommuniMerge.Models
{
    public class GroupMessageLink
    {
        public int GroupId { get; set; }
        public Group Group { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}