using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        Task<List<ConversationDto>> GetConversationsByMessageKeywordAsync(string userId, Dictionary<string, string> searchParms, bool isExactSearch, CancellationToken cancellationToken = default, int? type = null);

        Task<List<ConversationDto>> GetConversationsByUserAsync(string userId, PageSettings pageSettings, CancellationToken cancellationToken = default, int? type = null);
    }
}