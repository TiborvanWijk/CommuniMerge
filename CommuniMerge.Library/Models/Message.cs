namespace CommuniMerge.Library.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
        public User User { get; set; }
        public string? ReceiverId { get; set; }
        public GroupMessageLink? Group { get; set; }
    }
}
