using System;
using SugarChat.Core.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public class ConversationDataProvider : IConversationDataProvider
    {
        private readonly IRepository _repository;

        public ConversationDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Domain.Message>> GetPagingMessagesByConversationIdAsync(
            string conversationId,
            string nextReqMessageId = null,
            int count = 15,
            CancellationToken cancellationToken = default)
        {
            var nextReqMessage =
                await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == nextReqMessageId,
                    cancellationToken);
            var boundaryTime = nextReqMessage?.CreatedDate ?? DateTime.MaxValue;
            var messages = _repository.Query<Domain.Message>()
                .Where(x => x.GroupId == conversationId && x.CreatedDate < boundaryTime)
                .OrderByDescending(x => x.SentTime)
                .Take(count)
                .AsEnumerable();

            return messages;
        }
    }
}