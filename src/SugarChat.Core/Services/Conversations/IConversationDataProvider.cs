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
            int count = 15,
            CancellationToken cancellationToken = default);

        IEnumerable<Domain.Message> GetPagedMessagesByConversationIdAsync(string conversationId, int pageIndex = 0, int count = 15);
    }
}