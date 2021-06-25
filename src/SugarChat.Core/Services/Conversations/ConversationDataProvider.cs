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
            int pageIndex = 0,
            int count = 15,
            CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Domain.Message>().Where(x => x.GroupId == conversationId).OrderByDescending(x => x.SentTime);
            if (!string.IsNullOrEmpty(nextReqMessageId))
            {
                var nextReqMessage =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == nextReqMessageId,
                        cancellationToken);
                return query.Where(x => x.SentTime < nextReqMessage.SentTime).Take(count);
            }
            else if (pageIndex > 0)
            {
                return query.Skip((pageIndex - 1) * count).Take(count);
            }
            return query.Take(count).AsEnumerable();
        }
    }
}