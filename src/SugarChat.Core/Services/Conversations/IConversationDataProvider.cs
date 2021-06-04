using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public interface IConversationDataProvider : IDataProvider
    {
        Task<int> GetUserUnreadMessagesCountByGroupIdAndLastReadTimeAsync(string groupId, DateTimeOffset? lastReadTime, CancellationToken cancellationToken);

        Task<List<Domain.Message>> GetMessagesByGroupIdAsync(string groupId, CancellationToken cancellationToken);

        Task<Domain.Message> GetLastMessageByGroupIdAsync(string groupId, CancellationToken cancellationToken);

        Task<(List<Domain.Message> Messages, string NextReqMessageId)> GetPagingMessagesByConversationIdAsync(string conversationId, string nextReqMessageId = "", int count = 15, CancellationToken cancellationToken = default(CancellationToken));



    }
}
