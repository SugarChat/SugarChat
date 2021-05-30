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

        public async Task<IEnumerable<Domain.Message>> GetUnreadToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default)
        {
            var groupIdsIncludingBoth = GetGroupIdsIncludingUserAndFiend(userId, friendId);
            var groupId = GetGroupIdOfUserAndFiend(groupIdsIncludingBoth);
            var lastReadTime = (await _repository.SingleAsync<GroupUser>(o => o.GroupId == groupId, cancellationToken))
                .LastReadTime;
            var messages = _repository.Query<Domain.Message>()
                .Where(o => o.GroupId == groupId && o.SentTime > lastReadTime).OrderByDescending(o => o.SentTime);
            return await Task.FromResult(messages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId,
            CancellationToken cancellationToken = default)
        {
            var groupIds = await _repository.ToListAsync<GroupUser>(o => o.UserId == userId, cancellationToken);
            var unreadMessages = await _repository.ToListAsync<Domain.Message>(
                o => groupIds.Select(x => x.GroupId).Contains(o.GroupId) &&
                     o.SentTime > groupIds.SingleOrDefault(x => x.GroupId == o.GroupId).LastReadTime,
                cancellationToken);

            return await Task.FromResult(unreadMessages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllHistoryToUserWithFriendAsync(string userId,
            string friendId, CancellationToken cancellationToken = default)
        {
            var groupIdsIncludingBoth = GetGroupIdsIncludingUserAndFiend(userId, friendId);
            var groupId = GetGroupIdOfUserAndFiend(groupIdsIncludingBoth);
            var messages = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId)
                .OrderByDescending(o => o.SentTime);

            return await Task.FromResult(messages);
        }

        private IQueryable<string> GetGroupIdsIncludingUserAndFiend(string userId, string friendId)
        {
            var userGroupIds = _repository.Query<GroupUser>().Where(o => o.UserId == userId).Select(o => o.GroupId);
            var friendGroupIds = _repository.Query<GroupUser>().Where(o => o.UserId == friendId).Select(o => o.GroupId);
            var groupIdsIncludingBoth = userGroupIds.Intersect(friendGroupIds);
            return groupIdsIncludingBoth;
        }

        private string GetGroupIdOfUserAndFiend(IQueryable<string> groupIdsIncludingBoth)
        {
            var groupId = _repository.Query<GroupUser>().Where(o => groupIdsIncludingBoth.Contains(o.GroupId))
                .GroupBy(o => o.GroupId).SingleOrDefault(o => o.Count() == 2)?.First().GroupId;

            return groupId;
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

        public async Task<IEnumerable<Domain.Message>> GetMessagesOfGroupBeforeAsync(string messageId, int count,
            CancellationToken cancellationToken)
        {
            var message = await _repository.SingleAsync<Domain.Message>(o => o.Id == messageId, cancellationToken);
            var messages =
                _repository.Query<Domain.Message>()
                    .Where(o => o.GroupId == message.GroupId && o.SentTime < message.SentTime)
                    .OrderByDescending(o => o.SentTime).Take(count);

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetMessagesOfGroupAsync(string groupId, int count,
            CancellationToken cancellationToken)
        {
            var messages = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId)
                .OrderByDescending(o => o.SentTime).Take(count).ToList();

            return await Task.FromResult(messages);
        }

        public async Task<Domain.Message> GetLatestMessagesOfGroupAsync(string groupId,
            CancellationToken cancellationToken)
        {
            var messages = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId)
                .OrderByDescending(o => o.SentTime).FirstOrDefault();
            return await Task.FromResult(messages);
        }
    }
}