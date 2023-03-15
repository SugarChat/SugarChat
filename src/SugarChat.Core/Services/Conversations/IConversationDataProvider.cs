using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Paging;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationDataProvider : IDataProvider
    {
        Task<IEnumerable<Domain.Message>> GetPagedMessagesByConversationIdAsync(
            string conversationId,
            string nextReqMessageId = null,
            int pageIndex = 0,
            int count = 15,
            CancellationToken cancellationToken = default);

        Task<PagedResult<ConversationDto>> GetConversationListAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            IEnumerable<SearchParamDto> searchParams,
            IEnumerable<SearchMessageParamDto> searchByKeywordParams,
            int pageNum,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<PagedResult<ConversationDto>> GetUnreadConversationListAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            IEnumerable<SearchParamDto> searchParams,
            IEnumerable<SearchMessageParamDto> searchByKeywordParams,
            int pageNum,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}