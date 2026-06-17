using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationService : IService
    {
        Task<PagedResult<ConversationDto>> GetConversationListByUserIdAsync(GetConversationListRequest request,
            CancellationToken cancellationToken = default);

        Task<GetConversationProfileResponse> GetConversationProfileByIdAsync(GetConversationProfileRequest request,
            CancellationToken cancellationToken = default);

        Task<GetMessageListResponse> GetPagedMessagesByConversationIdAsync(GetMessageListRequest request,
            CancellationToken cancellationToken = default);

        Task<ConversationRemovedEvent> RemoveConversationByConversationIdAsync(RemoveConversationCommand command,
            CancellationToken cancellationToken = default);

        Task RemoveConversationByConversationIdAsync2(RemoveConversationCommand command, CancellationToken cancellationToken = default);

        Task<PagedResult<ConversationDto>> GetConversationByKeywordAsync(GetConversationByKeywordRequest request, CancellationToken cancellationToken);

        Task<PagedResult<ConversationDto>> GetUnreadConversationListByUserIdAsync(GetUnreadConversationListRequest request, CancellationToken cancellationToken = default);
    }
}