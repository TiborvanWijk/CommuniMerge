using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.CookieRepositories.Interfaces;
using CommuniMerge.Library.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace CommuniMerge.ApiServices
{
    public class AccountApiService : IAccountApiService
    {
        private readonly CookieContainer cookieContainer;
        private readonly HttpClientHandler handler;
        private readonly HttpClient client;
        private readonly ICookieRepository cookieRepository;

        public AccountApiService(ICookieRepository cookieRepository)
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            this.cookieRepository = cookieRepository;
        }
        public async Task<HttpResponseMessage> Login(HttpContext httpContext, LoginModel loginModel)
        {
            var cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            var jsonLoginModel = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");

            var result = await client.PostAsync("https://localhost:7129/Login", jsonLoginModel);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);

            return result;
        }

        public async Task<HttpResponseMessage> Register(HttpContext httpContext, RegisterModel registerModel)
        {
            var cookie = await cookieRepository.GetCookieByNameAsync(httpContext, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            var jsonRegisterModel = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json");

            var result = await client.PostAsync("https://localhost:7129/Register", jsonRegisterModel);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);

            return result;
        }
    }
}
