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


        public async Task<int> GetUserUnreadMessagesCountByGroupIdAndLastReadTimeAsync(string groupId, DateTimeOffset? lastReadTime, CancellationToken cancellationToken)
        {
            return await _repository.CountAsync<Domain.Message>(x => x.GroupId == groupId && (lastReadTime == null || x.SentTime > lastReadTime));
        }

        public async Task<List<Domain.Message>> GetMessagesByGroupIdAsync(string groupId, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Domain.Message>(x => x.GroupId == groupId, cancellationToken);
        }

        public async Task<Domain.Message> GetLastMessageByGroupIdAsync(string groupId, CancellationToken cancellationToken)
        {
            return (await _repository.ToListAsync<Domain.Message>(x => x.GroupId == groupId, cancellationToken)).OrderByDescending(x => x.SentTime).FirstOrDefault();
        }
    }
}
