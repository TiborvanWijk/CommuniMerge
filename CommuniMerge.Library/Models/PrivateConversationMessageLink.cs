namespace CommuniMerge.Library.Models
{
    public class PrivateConversationMessageLink
    {
        public int PrivateConversationId { get; set; }
        public PrivateConversationLink PrivateConversation { get; set; }
        public int MessageId { get; set; }
        public Message Message { get; set; }
    }
}
