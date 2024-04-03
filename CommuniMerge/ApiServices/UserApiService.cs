using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.CookieRepositories;
using CommuniMerge.CookieRepositories.Interfaces;
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




    }
}
