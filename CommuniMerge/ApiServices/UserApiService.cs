using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.CookieRepositories;
using CommuniMerge.CookieRepositories.Interfaces;
using CommuniMerge.Library.Models;
using System.Net;

namespace CommuniMerge.ApiServices
{
    public class UserApiService : IUserApiService
    {
        private readonly CookieContainer cookieContainer;
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;
        private readonly ICookieRepository cookieRepository;

        public UserApiService(ICookieRepository cookieRepository)
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            this.cookieRepository = cookieRepository;
        }



        public async Task<HttpResponseMessage> SendFriendRequest(HttpContext httpContext, string receiverUsername)
        {

            string cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            var result = await client.PostAsync($"https://localhost:7129/sendFriendRequest/{receiverUsername}", null);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }

        public async Task<HttpResponseMessage> GetAllFriends(HttpContext httpContext, bool withLatestMessage)
        {

            string cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            string url = withLatestMessage
                ? $"https://localhost:7129/api/User/friends?withLatestMessage={withLatestMessage}"
                : $"https://localhost:7129/api/User/friends";

            var result = await client.GetAsync(url);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }

        public async Task<HttpResponseMessage> GetAllFriendRequests(HttpContext httpContext)
        {

            string cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            string url = $"https://localhost:7129/api/User/friendrequests";

            var result = await client.GetAsync(url);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }

        public async Task<HttpResponseMessage> AcceptFriendRequest(HttpContext httpContext, string username)
        {

            string cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            string url = $"https://localhost:7129/acceptFriendRequest/{username}";

            var result = await client.PostAsync(url, null);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }

        public async Task<HttpResponseMessage> DeclineFriendRequest(HttpContext httpContext, string username)
        {
            string cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            string url = $"https://localhost:7129/declineFriendRequest/{username}";

            var result = await client.PostAsync(url, null);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }
    }
}
