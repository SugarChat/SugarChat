using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Services.Messages
{
    public class MessageDataProvider : IMessageDataProvider
    {

        private readonly IRepository _repository;

        public MessageDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(Domain.Message message, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddAsync(message, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.AddMessageFailed.WithParams(message.Id));
            }
        }

        public async Task UpdateAsync(Domain.Message message, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.UpdateAsync(message, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.UpdateMessageFailed.WithParams(message.Id));
            }
        }

        public async Task RemoveAsync(Domain.Message message, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(message, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.RemoveMessageFailed.WithParams(message.Id));
            }
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
            var lastReadTime =
                (await _repository.SingleAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                    cancellationToken))
                .LastReadTime;
            var messages = _repository.Query<Domain.Message>()
                .Where(o => o.GroupId == groupId && o.SentTime > (lastReadTime ?? DateTimeOffset.MinValue))
                .OrderByDescending(o => o.SentTime);
            return await Task.FromResult(messages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId,
            CancellationToken cancellationToken = default)
        {
            var groups = await _repository.ToListAsync<GroupUser>(o => o.UserId == userId, cancellationToken);
            var groupIds = groups.Select(x => x.GroupId);
            var messages =
                await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId), cancellationToken);
            var unreadMessages = messages.Where(o =>
                    o.SentTime > (groups.Single(x => x.GroupId == o.GroupId).LastReadTime ?? DateTimeOffset.MinValue))
                .ToList();

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

        private IEnumerable<string> GetGroupIdsIncludingUserAndFiend(string userId, string friendId)
        {
            var userGroupIds = _repository.Query<GroupUser>().Where(o => o.UserId == userId).Select(o => o.GroupId)
                .ToList();
            var friendGroupIds = _repository.Query<GroupUser>().Where(o => o.UserId == friendId).Select(o => o.GroupId)
                .ToList();
            var groupIdsIncludingBoth = userGroupIds.Intersect(friendGroupIds);
            return groupIdsIncludingBoth;
        }

        private string GetGroupIdOfUserAndFiend(IEnumerable<string> groupIdsIncludingBoth)
        {
            var groupId = _repository.Query<GroupUser>().Where(o => groupIdsIncludingBoth.Contains(o.GroupId))
                .AsEnumerable()
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

        public async Task<IEnumerable<Domain.Message>> GetUnreadMessagesFromGroupAsync(string userId, string groupId, string messageId = null, int count = 15,
            CancellationToken cancellationToken = default)
        {
            var unreadTime = (await _repository.SingleAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                cancellationToken)).LastReadTime;

            var query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && (unreadTime == null || o.SentTime > unreadTime));

            if (!string.IsNullOrEmpty(messageId))
            {
                var message =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == messageId,
                        cancellationToken);
                query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && (unreadTime == null || o.SentTime > unreadTime)
                                                                  && o.SentTime < message.SentTime);
            }

            var messages = query.OrderByDescending(o => o.SentTime)
                               .Take(count)
                               .AsEnumerable();
            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetAllMessagesFromGroupAsync(string groupId, string messageId = null, int count = 15,
            CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId);

            if (!string.IsNullOrEmpty(messageId))
            {
                var message =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == messageId,
                        cancellationToken);
                query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && o.SentTime < message.SentTime);
            }

            var messages = query.OrderByDescending(o => o.SentTime)
                               .Take(count)
                               .AsEnumerable();
            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetMessagesOfGroupBeforeAsync(string messageId, int count,
            CancellationToken cancellationToken = default)
        {
            var message = await _repository.SingleAsync<Domain.Message>(o => o.Id == messageId, cancellationToken);
            var messages =
                _repository.Query<Domain.Message>()
                    .Where(o => o.GroupId == message.GroupId && o.SentTime < message.SentTime)
                    .OrderByDescending(o => o.SentTime).Take(count);

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetMessagesOfGroupAsync(string groupId, int count,
            CancellationToken cancellationToken = default)
        {
            var messages = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId)
                .OrderByDescending(o => o.SentTime).Take(count).ToList();

            return await Task.FromResult(messages);
        }

        public async Task<Domain.Message> GetLatestMessageOfGroupAsync(string groupId,
            CancellationToken cancellationToken = default)
        {
            var message = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId)
                .OrderByDescending(o => o.SentTime).FirstOrDefault();
            return await Task.FromResult(message);
        }

        public async Task<IEnumerable<Domain.Message>> GetByGroupIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Domain.Message>(x => x.GroupId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<Domain.Message> messages, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync(messages, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId, CancellationToken cancellationToken = default)
        {
            var groups = await _repository.ToListAsync<GroupUser>(o => o.UserId == userId, cancellationToken);
            var groupIds = groups.Select(x => x.GroupId);
            var messages =
                await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId), cancellationToken);
            var unreadMessageCount = messages.Count(o =>
                    o.SentTime > (groups.Single(x => x.GroupId == o.GroupId).LastReadTime ?? DateTimeOffset.MinValue));

            return unreadMessageCount;
        }

        public async Task<IEnumerable<Domain.Message>> GetByGroupIdsAsync(string[] groupIds, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Domain.Message>(x => groupIds.Contains(x.GroupId), cancellationToken).ConfigureAwait(false);
        }
    }
}