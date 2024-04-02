using CommuniMerge.Library.Data.Dtos;

namespace CommuniMerge.ApiServices.Interfaces
{
    public interface IMessageApiService
    {
        Task<HttpResponseMessage> CreatePersonalMessage(HttpContext context, PersonalMessageCreateDto messageDto);
    }
}
