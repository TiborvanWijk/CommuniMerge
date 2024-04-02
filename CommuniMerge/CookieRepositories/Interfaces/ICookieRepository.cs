using System.Net;

namespace CommuniMerge.CookieRepositories.Interfaces
{
    public interface ICookieRepository
    {
        Task<string?> GetCookieByNameAsync(HttpContext context, string cookieName);
        Task AddBearerTokenAsCookieToContainer(CookieContainer cookieContainer, string? cookie);
        Task RemoveAllCookieOfContainer(CookieContainer cookieContainer);
    }
}
