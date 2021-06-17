using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Shared.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationService : IService
    {
        Task<GetConversationListResponse> GetConversationListByUserIdAsync(GetConversationListRequest request,
            CancellationToken cancellationToken = default);

        Task<GetConversationProfileResponse> GetConversationProfileByIdAsync(GetConversationProfileRequest request,
            CancellationToken cancellationToken = default);

        Task<GetMessageListResponse> GetPagedMessagesByConversationIdAsync(GetMessageListRequest request,
            CancellationToken cancellationToken = default);

        Task<ConversationRemovedEvent> RemoveConversationByConversationIdAsync(RemoveConversationCommand command,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<MessageDto>> GetPagedMessagesByConversationIdAsync(GetMessageListByPageIndexRequest request, CancellationToken cancellationToken = default);
    }
}