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
                .Where(o => o.GroupId == groupId && o.SentTime > (lastReadTime ?? DateTimeOffset.MinValue) && !o.IsRevoked)
                .OrderByDescending(o => o.SentTime);
            return await Task.FromResult(messages);
        }

        public async Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId, CancellationToken cancellationToken = default, int? type = null)
        {
            var groupUsers = (from a in _repository.Query<GroupUser>()
                              join b in _repository.Query<Group>() on a.GroupId equals b.Id
                              where a.UserId == userId && (b.Type == type || (type == 0 && b.Type == null))
                              select new
                              {
                                  a.Id,
                                  a.UserId,
                                  a.GroupId,
                                  a.Role,
                                  a.MessageRemindType,
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

        public async Task<(List<GroupUnReadCount>, int)> GetUnreadCountByGroupIdsAsync(string userId,
            IEnumerable<string> groupIds,
            Dictionary<string, List<string>> filterByGroupCustomProperties = null,
            Dictionary<string, List<string>> filterByGroupUserCustomProperties = null,
            Dictionary<string, List<string>> filterByMessageCustomProperties = null,
            CancellationToken cancellationToken = default
            )
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var groupIdsByFilter = new List<string>();
            if (filterByGroupCustomProperties != null)
            {
                var sb = new StringBuilder();
                foreach (var dic in filterByGroupCustomProperties)
                {
                    foreach (var value in dic.Value)
                    {
                        var _value = value.Replace("\\", "\\\\");
                        var _key = dic.Key.Replace("\\", "\\\\");
                        sb.Append($" || (Key==\"{_key}\" && Value==\"{_value}\")");
                    }
                }
                var where = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupCustomProperty>().Where(x => groupIds.Contains(x.GroupId)), sb.ToString().Substring(4));
                var groupCustomProperties = await _repository.ToListAsync(where, cancellationToken).ConfigureAwait(false);
                groupIdsByFilter = groupCustomProperties.Select(x => x.GroupId).Distinct().ToList();
            }
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync1 " + sw.ElapsedMilliseconds);
            sw.Restart();

            var userIdsByFilter = new List<string>();
            if (filterByGroupUserCustomProperties != null)
            {
                var groupUserIds = _repository.Query<GroupUser>().Where(x => groupIds.Contains(x.GroupId)).Select(x => x.Id).ToList();
                var sb = new StringBuilder();
                foreach (var dic in filterByGroupUserCustomProperties)
                {
                    foreach (var value in dic.Value)
                    {
                        var _value = value.Replace("\\", "\\\\");
                        var _key = dic.Key.Replace("\\", "\\\\");
                        sb.Append($" || (Key==\"{_key}\" && Value==\"{_value}\")");
                    }
                }
                var where = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUserCustomProperty>().Where(x => groupUserIds.Contains(x.Id)), sb.ToString().Substring(4));
                var groupUserCustomProperties = await _repository.ToListAsync(where, cancellationToken).ConfigureAwait(false);
                groupUserIds = groupUserCustomProperties.Select(x => x.GroupUserId).ToList();
                userIdsByFilter = (await _repository.ToListAsync<GroupUser>(x => groupUserIds.Contains(x.Id), cancellationToken).ConfigureAwait(false)).Select(x => x.UserId).ToList();
            }
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync2 " + sw.ElapsedMilliseconds);
            sw.Restart();

            var messageIdsByFilter = new List<string>();
            if (filterByMessageCustomProperties != null)
            {
                var sb = new StringBuilder();
                foreach (var dic in filterByMessageCustomProperties)
                {
                    foreach (var value in dic.Value)
                    {
                        var _value = value.Replace("\\", "\\\\");
                        var _key = dic.Key.Replace("\\", "\\\\");
                        sb.Append($" || (Key==\"{_key}\" && Value==\"{_value}\")");
                    }
                }
                var where = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<Domain.MessageCustomProperty>(), sb.ToString().Substring(4));
                var messageCustomProperties = await _repository.ToListAsync(where, cancellationToken).ConfigureAwait(false);
                messageIdsByFilter = messageCustomProperties.Select(x => x.MessageId).Distinct().ToList();
            }
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync3 " + sw.ElapsedMilliseconds);
            sw.Restart();

            var query = _repository.Query<GroupUser>().Where(x => x.UserId == userId);
            if (groupIds is not null && groupIds.Any())
            {
                query = query.Where(x => groupIds.Contains(x.GroupId));
            }
            if (groupIdsByFilter.Any())
            {
                query = query.Where(x => !groupIdsByFilter.Contains(x.GroupId));
            }
            var groupUsers = await _repository.ToListAsync(query, cancellationToken).ConfigureAwait(false);
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync4 " + sw.ElapsedMilliseconds);
            sw.Restart();

            var groupIdsByUser = groupUsers.Select(x => x.GroupId).ToList();
            var queryByMessage = _repository.Query<Domain.Message>().Where(x => groupIdsByUser.Contains(x.GroupId) && x.SentBy != userId);
            if (userIdsByFilter.Any())
            {
                queryByMessage = queryByMessage.Where(x => !userIdsByFilter.Contains(x.SentBy));
            }
            if (messageIdsByFilter.Any())
            {
                queryByMessage = queryByMessage.Where(x => !messageIdsByFilter.Contains(x.Id));
            }
            var messages = queryByMessage.Select(x => new { x.GroupId, x.Id, x.SentTime }).ToList();
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync5 " + sw.ElapsedMilliseconds);
            sw.Restart();
            messages = (from a in groupUsers
                        join b in messages on a.GroupId equals b.GroupId
                        where b.SentTime > a.LastReadTime || a.LastReadTime is null
                        select b).ToList();
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync6 " + sw.ElapsedMilliseconds);
            sw.Restart();

            var messagesGroup = messages.GroupBy(x => x.GroupId).ToList();
            sw.Stop();
            Serilog.Log.Warning("GetUnreadCountByGroupIdsAsync7 " + sw.ElapsedMilliseconds);
            List<GroupUnReadCount> groupUnReadCounts = new List<GroupUnReadCount>();
            messagesGroup.ForEach(x => { groupUnReadCounts.Add(new GroupUnReadCount { GroupId = x.Key, UnReadCount = x.Count() }); });

            return (groupUnReadCounts, messages.Count());
        }

        public async Task<IEnumerable<Domain.Message>> GetByGroupIdsAsync(string[] groupIds, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Domain.Message>(x => groupIds.Contains(x.GroupId) && !x.IsRevoked, cancellationToken).ConfigureAwait(false);
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

        public async Task<IEnumerable<MessageCountGroupByGroupId>> GetMessageUnreadCountGroupByGroupIdsAsync(IEnumerable<string> groupIds, string userId, PageSettings pageSettings, CancellationToken cancellationToken = default)
        {
            if (groupIds == null || !groupIds.Any())
                return new List<MessageCountGroupByGroupId>();

            var groupUsers = pageSettings == null ? await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId) && x.UserId == userId)
                : (await _repository.ToPagedListAsync<GroupUser>(pageSettings, x => groupIds.Contains(x.GroupId) && x.UserId == userId)).Result;

            var groupIdsByGroupUser = groupUsers.Select(x => x.GroupId).ToList();
            var messages = _repository.Query<Domain.Message>().Where(x => groupIdsByGroupUser.Contains(x.GroupId) && x.SentBy != userId).Select(x => new { x.GroupId, x.SentTime }).ToList();
            var messageGroups = (from a in groupUsers
                                 join b in messages on a.GroupId equals b.GroupId
                                 where (b.SentTime > a.LastReadTime || a.LastReadTime == null)
                                 group b by b.GroupId into c
                                 select new
                                 {
                                     GroupId = c.Key,
                                     UnReadCount = c.Count(),
                                     LastMessageSentTime = c.Max(x => x.SentTime)
                                 }).ToList();

            List<MessageCountGroupByGroupId> result = new List<MessageCountGroupByGroupId>();
            foreach (var groupUser in groupUsers)
            {
                var messageGroup = messageGroups.FirstOrDefault(x => x.GroupId == groupUser.GroupId);
                var messageCountGroupByGroupId = new MessageCountGroupByGroupId
                {
                    GroupId = groupUser.GroupId
                };
                if (messageGroup != null)
                {
                    messageCountGroupByGroupId.Count = messageGroup.UnReadCount;
                    messageCountGroupByGroupId.LastSentTime = messageGroup.LastMessageSentTime;
                }
                result.Add(messageCountGroupByGroupId);
            }
            result = result.OrderByDescending(x => x.Count).ToList();
            return result;
        }

        public async Task<Domain.Message> GetLastMessageBygGroupIdAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return _repository.Query<Domain.Message>().Where(x => x.GroupId == groupId && !x.IsRevoked).OrderByDescending(x => x.SentTime).FirstOrDefault();
        }

        public async Task<IEnumerable<Domain.Message>> GetLastMessageForGroupsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var messageIds = (from a in _repository.Query<Domain.Message>()
                              where groupIds.Contains(a.GroupId)
                              orderby a.SentTime descending
                              group a by a.GroupId into b
                              select new { b.First().Id }).ToList().Select(x => x.Id).ToList();

            var messages = await _repository.ToListAsync<Domain.Message>(x => messageIds.Contains(x.Id), cancellationToken).ConfigureAwait(false);
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

    public class GroupUnReadCount
    {
        public string GroupId { get; set; }
        public int UnReadCount { get; set; }
    }
}