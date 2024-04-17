namespace CommuniMerge.ApiServices.Interfaces
{
    public interface IApiService
    {
        Task<HttpResponseMessage> SendHttpRequest<T>(HttpContext context, string endpoint, HttpMethod httpMethod, T? content, bool sendAsFormData = false);
    }
}
