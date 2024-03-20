namespace CommuniMerge.Library.Models
{
    public class PrivateConversationLink
    {
        public int Id { get; set; }
        public string User1Id { get; set; }
        public User User1 { get; set; }
        public string User2Id { get; set; }
        public User User2 { get; set; }
        public ICollection<PrivateConversationMessageLink> PrivateConversationMessageLinks { get; set; }
    }
}
