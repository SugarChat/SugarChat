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

        public async Task<IEnumerable<Domain.Message>> GetPagedMessagesByConversationIdAsync(
            string conversationId,
            string nextReqMessageId = null,
            int count = 15,
            CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Domain.Message>().Where(x => x.GroupId == conversationId);
            if (!string.IsNullOrEmpty(nextReqMessageId))
            {
                var nextReqMessage =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == nextReqMessageId,
                        cancellationToken);
                query = _repository.Query<Domain.Message>().Where(x => x.GroupId == conversationId && x.CreatedDate < nextReqMessage.CreatedDate);
            }
            var messages = query.OrderByDescending(x => x.SentTime)
                .Take(count)
                .AsEnumerable();
            return messages;
        }
    }
}