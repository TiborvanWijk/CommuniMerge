using CommuniMerge.Library.Models;

namespace CommuniMerge.ViewModels
{
    public class LoginView
    {
        public LoginModel LoginModel { get; set; }
        public string? FeedbackMessage { get; set; } = null;
    }
}
