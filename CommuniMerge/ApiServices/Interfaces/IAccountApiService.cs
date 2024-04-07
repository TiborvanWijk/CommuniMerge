using CommuniMerge.Library.Models;

namespace CommuniMerge.ApiServices.Interfaces
{
    public interface IAccountApiService
    {
        Task<HttpResponseMessage> Login(HttpContext httpContext, LoginModel loginModel);
        Task<HttpResponseMessage> Register(HttpContext httpContext, RegisterModel registerModel);
    }
}
