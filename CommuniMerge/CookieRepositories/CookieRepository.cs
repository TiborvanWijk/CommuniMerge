using CommuniMerge.CookieRepositories.Interfaces;
using System.Net;

namespace CommuniMerge.CookieRepositories
{
    public class CookieRepository : ICookieRepository
    {

        public async Task<string?> GetCookieByNameAsync(HttpContext context, string cookieName)
        {
            var cookie = context.Request.Cookies[cookieName];
            return cookie;
        }

        public async Task AddBearerTokenAsCookieToContainer(CookieContainer cookieContainer, string? cookie)
        {
            if (!string.IsNullOrEmpty(cookie))
            {
                cookieContainer.Add(new Uri("https://localhost:7129"), new Cookie("BearerToken", cookie));
            }
        }

        public async Task RemoveAllCookieOfContainer(CookieContainer cookieContainer)
        {
            var cookies = cookieContainer.GetAllCookies().ToList();
            foreach (var cookie in cookies)
            {
                cookie.Expired = true;
            }
        }

    }
}
