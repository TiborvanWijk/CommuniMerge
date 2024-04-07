using CommuniMerge.Library.Models;

namespace CommuniMerge.ViewModels
{
    public class RegisterView
    {
        public RegisterModel RegisterModel { get; set; }
        public string? FeedbackMessage { get; set; } = null;
    }
}
