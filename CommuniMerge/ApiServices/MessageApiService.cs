using Azure.Core;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.CookieRepositories.Interfaces;
using CommuniMerge.Library.Data.Dtos;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace CommuniMerge.ApiServices
{
    public class MessageApiService : IMessageApiService
    {
        private readonly HttpClient client;
        private readonly CookieContainer cookieContainer;
        private readonly HttpClientHandler handler;
        private readonly ICookieRepository cookieRepository;

        public MessageApiService(ICookieRepository cookieRepository)
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            this.cookieRepository = cookieRepository;
        }


        public async Task<HttpResponseMessage> CreatePersonalMessage(HttpContext context, PersonalMessageCreateDto messageDto)
        {

            var cookie = await cookieRepository.GetCookieByNameAsync(context, "BearerToken");

            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);

            var jsonContent = new StringContent(JsonConvert.SerializeObject(messageDto), Encoding.UTF8, "application/json");

            var result = await client.PostAsync("https://localhost:7129/createPersonalMessage", jsonContent);

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }


    }
}
