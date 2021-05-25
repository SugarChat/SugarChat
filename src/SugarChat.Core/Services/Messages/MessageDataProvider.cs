using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Messages
{
    public class MessageDataProvider : IMessageDataProvider
    {
        private readonly IRepository _repository;

        public MessageDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.AddAsync(message, cancellation);
        }

        public async Task UpdateAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.UpdateAsync(message, cancellation);
        }

        public async Task RemoveAsync(Domain.Message message, CancellationToken cancellation)
        {
            await _repository.RemoveAsync(message, cancellation);
        }

        public async Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Domain.Message>(o => o.Id == id, cancellationToken);
        }

        public Task<IEnumerable<Domain.Message>> GetUnreadToUserFromFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default)
        {
            var unreadMessageIds = _repository.Query<MessageUnread>().Where(o => o.UserId == userId)
                .Select(o => o.MessageId);
            var unreadMessages = _repository.Query<Domain.Message>().Where(o => unreadMessageIds.Contains(o.Id));
            var fromFriends = unreadMessages.Where(o => o.SentBy == friendId).AsEnumerable();
            return Task.FromResult(fromFriends);
        }

        public Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId,
            CancellationToken cancellationToken = default)
        {
            var unreadMessageIds = _repository.Query<MessageUnread>().Where(o => o.UserId == userId)
                .Select(o => o.MessageId);
            var unreadMessages = _repository.Query<Domain.Message>().Where(o => unreadMessageIds.Contains(o.Id))
                .AsEnumerable();
            return Task.FromResult(unreadMessages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllHistoryToUserFromFriendAsync(string userId,
            string friendId,
            CancellationToken cancellationToken = default)
        {
            var userGroup = _repository.Query<GroupUser>().Where(o => o.UserId == userId);
            var friendGroup = _repository.Query<GroupUser>().Where(o => o.UserId == friendId);
            var bothGroup = userGroup.Intersect(friendGroup);
            var groupId = bothGroup.GroupBy(o => o.GroupId).Single(o => o.Count() == 2).First().GroupId;
            var messages =
                await _repository.ToListAsync<Domain.Message>(o => o.GroupId == groupId && o.SentBy == friendId,
                    cancellationToken);

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetAllHistoryToUserAsync(string userId,
            CancellationToken cancellationToken = default)
        {
            var groupIds =
                (await _repository.ToListAsync<GroupUser>(o => o.UserId == userId, cancellationToken)).Select(o =>
                    o.GroupId);
            var messages =
                (await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId), cancellationToken))
                .OrderBy(o => o.GroupId).ThenByDescending(o => o.SentTime);

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetUnreadToUserFromGroupAsync(string userId, string groupId,
            CancellationToken cancellationToken)
        {
            var unreadTime = (await _repository.SingleAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                cancellationToken)).LastReadTime;

            var messages =
                (await _repository.ToListAsync<Domain.Message>(
                    o => o.GroupId == groupId && (unreadTime == null || o.SentTime > unreadTime),
                    cancellationToken)).OrderByDescending(o => o.SentTime);

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetAllToUserFromGroupAsync(string userId, string groupId,
            CancellationToken cancellationToken)
        {
            var messages =
                await _repository.ToListAsync<Domain.Message>(o => o.GroupId == groupId && o.SentBy != userId,
                    cancellationToken);

            return messages;
        }
    }
}