using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationService : IService
    {
        Task<GetConversationListResponse> GetConversationListByUserIdAsync(GetConversationListRequest request, CancellationToken cancellationToken);
        Task<GetConversationProfileResponse> GetConversationProfileByIdAsync(GetConversationProfileRequest request, CancellationToken cancellationToken);       
        Task SetMessageAsReadByConversationIdAsync(SetMessageAsReadCommand command, CancellationToken cancellationToken);
        Task<GetMessageListResponse> GetPagingMessagesByConversationIdAsync(GetMessageListRequest request, CancellationToken cancellationToken);
    }
}
