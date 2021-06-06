using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationDataProvider : IDataProvider
    {    
        Task<(List<Domain.Message> Messages, string NextReqMessageId)> GetPagingMessagesByConversationIdAsync(
            string conversationId, string nextReqMessageId = "", int count = 15,
            CancellationToken cancellationToken = default);
    }
}