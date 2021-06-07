using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationDataProvider : IDataProvider
    {
        Task<IEnumerable<Domain.Message>> GetPagingMessagesByConversationIdAsync(
            string conversationId,
            string nextReqMessageId = null,
            int count = 15,
            CancellationToken cancellationToken = default);
    }
}