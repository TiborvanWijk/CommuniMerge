using CommuniMerge.Library.Data.Dtos;

namespace Communimerge.Api.Hubs.ClientInterfaces
{
    public interface IGroupClient
    {
        Task SuccesCreatingGroup(GroupDto groupDto);
    }
}
