using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.CookieRepositories.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace CommuniMerge.ApiServices
{
    public class ApiService : IApiService
    {

        private readonly HttpClient client;
        private readonly CookieContainer cookieContainer;
        private readonly HttpClientHandler handler;
        private readonly ICookieRepository cookieRepository;
        private readonly string apiUrl;

        public ApiService(ICookieRepository cookieRepository, IConfiguration configuration)
        {
            cookieContainer = new CookieContainer();
            handler = new HttpClientHandler { CookieContainer = cookieContainer };
            client = new HttpClient(handler);
            this.cookieRepository = cookieRepository;
            apiUrl = configuration["ApiSettings:url"];
        }

        public async Task<HttpResponseMessage> SendHttpRequest<T>(HttpContext context, string endpoint, HttpMethod httpMethod, T? content, bool sendAsFormData = false)
        {
            var cookie = await cookieRepository.GetCookieByNameAsync(context, "BearerToken");
            await cookieRepository.AddBearerTokenAsCookieToContainer(cookieContainer, cookie);
            string url = $"{apiUrl}{endpoint}";

            HttpContent httpContent = null;
            if (content != null)
            {
                if (sendAsFormData)
                {
                    var formData = new FormUrlEncodedContent(JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(content)));
                    httpContent = formData;
                }
                else
                {
                    httpContent = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json");
                }
            }

            HttpResponseMessage result = null;
            switch (httpMethod.Method)
            {
                case "GET":
                    result = await client.GetAsync(url);
                    break;
                case "POST":
                    result = await client.PostAsync(url, httpContent);
                    break;
                case "PUT":
                    result = await client.PutAsync(url, httpContent);
                    break;
                case "PATCH":
                    result = await client.PatchAsync(url, httpContent);
                    break;
                case "DELETE":
                    result = await client.DeleteAsync(url);
                    break;
                default:
                    throw new NotSupportedException($"HTTP method '{httpMethod.Method}' is not supported.");
            }

            await cookieRepository.RemoveAllCookieOfContainer(cookieContainer);
            return result;
        }
    }
}
