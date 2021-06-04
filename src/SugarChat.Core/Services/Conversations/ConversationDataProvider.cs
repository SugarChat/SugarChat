using SugarChat.Core.IRepositories;
using System;
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

        public async Task<int> GetUserUnreadMessagesCountByGroupIdAndLastReadTimeAsync(string groupId,
            DateTimeOffset? lastReadTime, CancellationToken cancellationToken)
        {
            return await _repository
                .CountAsync<Domain.Message>(
                    x => x.GroupId == groupId && (lastReadTime == null || x.SentTime > lastReadTime), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<List<Domain.Message>> GetMessagesByGroupIdAsync(string groupId,
            CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Domain.Message>(x => x.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Domain.Message> GetLastMessageByGroupIdAsync(string groupId,
            CancellationToken cancellationToken = default)
        {
            return (await _repository.ToListAsync<Domain.Message>(x => x.GroupId == groupId, cancellationToken)
                    .ConfigureAwait(false))
                .OrderByDescending(x => x.SentTime).FirstOrDefault();
        }

        public async Task<(List<Domain.Message> Messages, string NextReqMessageId)>
            GetPagingMessagesByConversationIdAsync(string conversationId, string nextReqMessageId = "", int count = 15,
                CancellationToken cancellationToken = default)
        {
            var messages = new List<Domain.Message>();

            if (string.IsNullOrEmpty(nextReqMessageId))
            {
                messages = _repository.Query<Domain.Message>().Where(x => x.GroupId == conversationId)
                    .OrderByDescending(x => x.SentTime)
                    .Take(count)
                    .ToList();
            }
            else
            {
                var nextReqMessage =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == nextReqMessageId);
                messages = _repository.Query<Domain.Message>().Where(x =>
                        x.GroupId == conversationId && x.CreatedDate < nextReqMessage.CreatedDate)
                    .OrderByDescending(x => x.SentTime)
                    .Take(count)
                    .ToList();
            }

            return (messages, messages?.Last()?.Id);
        }
    }
}