using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.MessageCustomProperties;
using System.Diagnostics;
using Serilog;

namespace SugarChat.Core.Services.Messages
{
    public class MessageDataProvider : IMessageDataProvider
    {

        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IMessageCustomPropertyDataProvider _messageCustomPropertyDataProvider;

        public MessageDataProvider(IRepository repository, IMapper mapper, IGroupUserDataProvider groupUserDataProvider, IMessageCustomPropertyDataProvider messageCustomPropertyDataProvider)
        {
            _repository = repository;
            _mapper = mapper;
            _groupUserDataProvider = groupUserDataProvider;
            _messageCustomPropertyDataProvider = messageCustomPropertyDataProvider;
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
                .Where(o => o.GroupId == groupId && o.SentTime > (lastReadTime ?? DateTimeOffset.MinValue) && !o.IsRevoked)
                .OrderByDescending(o => o.SentTime);
            return await Task.FromResult(messages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId, int groupType, CancellationToken cancellationToken = default)
        {
            var groupUsers = (from a in _repository.Query<GroupUser>()
                              join b in _repository.Query<Group>() on a.GroupId equals b.Id
                              where a.UserId == userId && b.Type == groupType
                              select new
                              {
                                  a.Id,
                                  a.UserId,
                                  a.GroupId,
                                  a.LastReadTime
                              }).ToList();
            var groupIds = groupUsers.Select(x => x.GroupId);
            var messages =
                await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId), cancellationToken);
            var unreadMessages = messages.Where(o => o.SentBy != userId &&
                    o.SentTime > (groupUsers.Single(x => x.GroupId == o.GroupId).LastReadTime ?? DateTimeOffset.MinValue) && !o.IsRevoked)
                .ToList();

            return await Task.FromResult(unreadMessages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllHistoryToUserWithFriendAsync(string userId,
            string friendId, CancellationToken cancellationToken = default)
        {
            var groupIdsIncludingBoth = GetGroupIdsIncludingUserAndFiend(userId, friendId);
            var groupId = GetGroupIdOfUserAndFiend(groupIdsIncludingBoth);
            var messages = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && !o.IsRevoked)
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
                (await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId) && !o.IsRevoked, cancellationToken))
                .OrderBy(o => o.GroupId).ThenByDescending(o => o.SentTime);

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetUnreadMessagesFromGroupAsync(string userId, string groupId, string messageId = null, int? count = 15,
            CancellationToken cancellationToken = default)
        {
            var unreadTime = (await _repository.SingleAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                cancellationToken)).LastReadTime;

            var query = _repository.Query<Domain.Message>().Where(o => o.SentBy != userId && o.GroupId == groupId && (unreadTime == null || o.SentTime > unreadTime) && !o.IsRevoked);

            if (!string.IsNullOrEmpty(messageId))
            {
                var message =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == messageId,
                        cancellationToken);
                query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && (unreadTime == null || o.SentTime > unreadTime)
                                                                  && o.SentTime < message.SentTime);
            }

            IEnumerable<Domain.Message> messages;

            if (count != null)
            {
                messages = query.OrderByDescending(o => o.SentTime)
                                .Take(count.Value)
                                .AsEnumerable();
            }
            else
            {
                messages = query.OrderByDescending(o => o.SentTime)
                                 .AsEnumerable();
            }

            return messages;
        }

        public async Task<IEnumerable<Domain.Message>> GetAllMessagesFromGroupAsync(string groupId, int index = 0, string messageId = null, int count = 0,
            CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && !o.IsRevoked).OrderByDescending(o => o.SentTime).AsEnumerable();

            if (!string.IsNullOrEmpty(messageId))
            {
                var message =
                    await _repository.SingleOrDefaultAsync<Domain.Message>(x => x.Id == messageId,
                        cancellationToken);
                query = query.Where(o => o.SentTime < message.SentTime);
            }
            else if (index != 0)
            {
                query = query.Skip((index - 1) * count);
            }
            if (count != 0)
            {
                query = query.Take(count);
            }
            return query;
        }

        public async Task<IEnumerable<Domain.Message>> GetMessagesOfGroupBeforeAsync(string messageId, int count,
            CancellationToken cancellationToken = default)
        {
            var message = await _repository.SingleAsync<Domain.Message>(o => o.Id == messageId, cancellationToken);
            var messages =
                _repository.Query<Domain.Message>()
                    .Where(o => o.GroupId == message.GroupId && o.SentTime < message.SentTime && !o.IsRevoked)
                    .OrderByDescending(o => o.SentTime).Take(count);

            return messages;
        }

        public async Task<PagedResult<Domain.Message>> GetMessagesOfGroupAsync(string groupId, PageSettings pageSettings, DateTimeOffset? fromDate, CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && !o.IsRevoked);
            if (fromDate is not null)
            {
                query = query.Where(x => x.SentTime >= fromDate);
            }
            query = query.OrderByDescending(o => o.SentTime);
            var result = new PagedResult<Domain.Message>();
            if (pageSettings is not null)
            {
                result = await _repository.ToPagedListAsync(pageSettings, query, cancellationToken);
            }
            else
            {
                var messages = query.ToArray();
                result.Result = messages;
                result.Total = messages.Count();
            }
            return result;
        }

        public async Task<Domain.Message> GetLatestMessageOfGroupAsync(string groupId,
            CancellationToken cancellationToken = default)
        {
            var message = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId && !o.IsRevoked)
                .OrderByDescending(o => o.SentTime).FirstOrDefault();
            return await Task.FromResult(message);
        }

        public async Task<IEnumerable<Domain.Message>> GetByGroupIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Domain.Message>(x => x.GroupId == id && !x.IsRevoked, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<Domain.Message> messages, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync(messages, cancellationToken).ConfigureAwait(false);
        }

        public async Task<(List<GroupUnreadCount>, int)> GetUnreadCountByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<GroupUser>().Where(x => x.UserId == userId);
            if (groupIds != null && groupIds.Any())
            {
                query = query.Where(x => groupIds.Contains(x.GroupId));
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var groupUsers = await _repository.ToListAsync(query, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetUnreadCountByGroupIdsAsync run{@Ms}", stopwatch.ElapsedMilliseconds);

            var groupUnreadCounts = new List<GroupUnreadCount>();
            foreach (var groupUser in groupUsers)
            {
                groupUnreadCounts.Add(new GroupUnreadCount { GroupId = groupUser.GroupId, UnreadCount = groupUser.UnreadCount });
            }

            return (groupUnreadCounts, groupUsers.Sum(x => x.UnreadCount));
        }

        public async Task<IEnumerable<Domain.Message>> GetUserUnreadMessagesByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var groups = await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId) && x.UserId == userId);
            var messages = await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId) && o.SentBy != userId && !o.IsRevoked, cancellationToken);
            var unreadMessages = messages.Where(o => o.SentTime > (groups.Single(x => x.GroupId == o.GroupId).LastReadTime ?? DateTimeOffset.MinValue));
            return unreadMessages;
        }

        public async Task<IEnumerable<Domain.Message>> GetMessagesByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var messages = await _repository.ToListAsync<Domain.Message>(x => groupIds.Contains(x.GroupId) && !x.IsRevoked, cancellationToken);
            return messages;
        }

        public async Task<IEnumerable<UnreadCountAndLastMessageByGroupId>> GetUnreadCountAndLastMessageByGroupIdsAsync(string userId,
            IEnumerable<string> groupIds,
            PageSettings pageSettings,
            int groupType,
            CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var groupUsers = await _groupUserDataProvider.GetByUserIdAsync(userId, groupIds, groupType, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync1 run{@Ms}", stopwatch.ElapsedMilliseconds);

            if (!groupUsers.Any())
            {
                return new List<UnreadCountAndLastMessageByGroupId>();
            }

            var groupIdsByGroupUser = groupUsers.Select(x => x.GroupId).ToList();

            stopwatch.Restart();
            var (groupUnreadCounts, count) = await GetUnreadCountByGroupIdsAsync(userId, groupIdsByGroupUser, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync2 run{@Ms}", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var lastMessages = await GetLastMessageForGroupsAsync(groupIdsByGroupUser, cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync3 run{@Ms}", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var messageCustomProperties = await _messageCustomPropertyDataProvider.GetPropertiesByMessageIds(lastMessages.Select(x => x.Id), cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync4 run{@Ms}", stopwatch.ElapsedMilliseconds);

            var unreadCountAndLastMessageByGroupIds = new List<UnreadCountAndLastMessageByGroupId>();

            stopwatch.Restart();
            foreach (var groupUnreadCount in groupUnreadCounts)
            {
                var lastMessage = lastMessages.FirstOrDefault(x => x.GroupId == groupUnreadCount.GroupId);
                var unreadCountAndLastMessageByGroupId = new UnreadCountAndLastMessageByGroupId
                {
                    GroupId = groupUnreadCount.GroupId,
                    UnreadCount = groupUnreadCount.UnreadCount
                };
                if (lastMessage != null)
                {
                    unreadCountAndLastMessageByGroupId.LastMessage = _mapper.Map<MessageDto>(lastMessage);
                    unreadCountAndLastMessageByGroupId.LastSentTime = lastMessage.SentTime;
                    var _messageCustomProperties = messageCustomProperties.Where(x => x.MessageId == lastMessage.Id).ToList();
                    unreadCountAndLastMessageByGroupId.LastMessage.CustomProperties = _messageCustomProperties.Select(x => new { x.Key, x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                }
                unreadCountAndLastMessageByGroupIds.Add(unreadCountAndLastMessageByGroupId);
            }
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync5 run{@Ms}", stopwatch.ElapsedMilliseconds);

            if (pageSettings == null)
            {
                return unreadCountAndLastMessageByGroupIds
                        .OrderByDescending(x => x.UnreadCount)
                        .ThenByDescending(x => x.LastSentTime)
                        .ToList();
            }
            else
            {
                return unreadCountAndLastMessageByGroupIds
                        .OrderByDescending(x => x.UnreadCount)
                        .ThenByDescending(x => x.LastSentTime)
                        .Skip(pageSettings.PageSize * (pageSettings.PageNum - 1))
                        .Take(pageSettings.PageSize).ToList();
            }
        }

        public async Task<Domain.Message> GetLastMessageBygGroupIdAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return _repository.Query<Domain.Message>().Where(x => x.GroupId == groupId && !x.IsRevoked).OrderByDescending(x => x.SentTime).FirstOrDefault();
        }

        public async Task<IEnumerable<Domain.Message>> GetLastMessageForGroupsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var messageIds = (from a in _repository.Query<Domain.Message>()
                              where groupIds.Contains(a.GroupId)
                              orderby a.SentTime descending
                              group a by a.GroupId into b
                              select new { b.First().Id }).ToList().Select(x => x.Id).ToList();
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetLastMessageForGroupsAsync1 run{@Ms}", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var messages = await _repository.ToListAsync<Domain.Message>(x => messageIds.Contains(x.Id), cancellationToken).ConfigureAwait(false);
            stopwatch.Stop();
            Log.Information("MessageDataProvider.GetLastMessageForGroupsAsync2 run{@Ms}", stopwatch.ElapsedMilliseconds);

            var result = new List<Domain.Message>();
            foreach (var groupId in groupIds)
            {
                var message = messages.Where(x => x.GroupId == groupId).FirstOrDefault();
                if (message != null)
                {
                    result.Add(message);
                }
            }
            return result;
        }

        public async Task UpdateRangeAsync(IEnumerable<Domain.Message> messages, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateRangeAsync(messages, cancellationToken);
        }

        public async Task<IEnumerable<Domain.Message>> GetListByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Domain.Message>(x => ids.Contains(x.Id) && !x.IsRevoked, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetCountAsync(Expression<Func<Domain.Message, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Domain.Message>> GetListAsync(PageSettings pageSettings, Expression<Func<Domain.Message, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return (await _repository.ToPagedListAsync(pageSettings, predicate, cancellationToken).ConfigureAwait(false)).Result;
        }
    }

    public class GroupUnreadCount
    {
        public string GroupId { get; set; }
        public int UnreadCount { get; set; }
    }
}