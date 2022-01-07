using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using MongoDB.Driver.Linq;

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
            var unreadMessages = messages.Where(o => o.SentBy != userId &&
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

        public async Task<IEnumerable<Domain.Message>> GetUnreadMessagesFromGroupAsync(string userId, string groupId, string messageId = null, int? count = 15,
            CancellationToken cancellationToken = default)
        {
            var unreadTime = (await _repository.SingleAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId,
                cancellationToken)).LastReadTime;

            var query = _repository.Query<Domain.Message>().Where(o => o.SentBy != userId && o.GroupId == groupId && (unreadTime == null || o.SentTime > unreadTime));

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
            var query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId).OrderByDescending(o => o.SentTime).AsEnumerable();

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
                    .Where(o => o.GroupId == message.GroupId && o.SentTime < message.SentTime)
                    .OrderByDescending(o => o.SentTime).Take(count);

            return messages;
        }

        public async Task<PagedResult<Domain.Message>> GetMessagesOfGroupAsync(string groupId, PageSettings pageSettings, DateTimeOffset? fromDate, CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Domain.Message>().Where(o => o.GroupId == groupId);
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

        class MessageCount
        {
            public int Count { get; set; }
        }

        public async Task<int> GetUnreadMessageCountAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var groupUsers = await _repository.ToListAsync<GroupUser>(o => o.UserId == userId, cancellationToken);
            if (groupIds.Any())
            {
                groupUsers = groupUsers.Where(x => groupIds.Contains(x.GroupId)).ToList();
            }
            var _groupIds = groupUsers.Select(x => x.GroupId);
            if (_groupIds.Count() == 0) return 0;

            List<string> stages = new List<string>();
            var lookup = GetLookup(userId);
            var match = GetMatch(userId, _groupIds);
            string project1 = "{$project:{Count:{$size:'$stockdata'}}}";
            string group = "{$group:{_id:null,Count:{$sum:'$Count'}}}";
            string project2= "{$project:{_id:0}}";
            stages.Add(match);
            stages.Add(lookup);
            stages.Add(project1);
            stages.Add(group);
            stages.Add(project2);

            var bsonDocuments = await (await _repository.GetAggregate<GroupUser>(stages, cancellationToken)).ToListAsync(cancellationToken);
            if (bsonDocuments.Count() == 0)
                return 0;

            var MessageCount = BsonSerializer.Deserialize<MessageCount>(bsonDocuments.FirstOrDefault());
            return MessageCount.Count;
        }

        public async Task<IEnumerable<Domain.Message>> GetByGroupIdsAsync(string[] groupIds, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Domain.Message>(x => groupIds.Contains(x.GroupId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Domain.Message>> GetUserUnreadMessagesByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var groups = await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId) && x.UserId == userId);
            var messages = await _repository.ToListAsync<Domain.Message>(o => groupIds.Contains(o.GroupId) && o.SentBy != userId, cancellationToken);
            var unreadMessages = messages.Where(o => o.SentTime > (groups.Single(x => x.GroupId == o.GroupId).LastReadTime ?? DateTimeOffset.MinValue));
            return unreadMessages;
        }

        public async Task<IEnumerable<Domain.Message>> GetMessagesByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var messages = await _repository.ToListAsync<Domain.Message>(x => groupIds.Contains(x.GroupId), cancellationToken);
            return messages;
        }

        public async Task<IEnumerable<MessageCountGroupByGroupId>> GetMessageUnreadCountGroupByGroupIdsAsync(IEnumerable<string> groupIds, string userId, PageSettings pageSettings, CancellationToken cancellationToken = default)
        {
            List<string> stages = new List<string>();
            var lookup1 = GetLookup(userId);
            var lookup2 = @"
{
    $lookup:{
        from:'Message',
        let:{groupUser_GroupId:'$GroupId'},
        pipeline:[
            {$match:
                {$expr:
                    {$eq:['$GroupId','$$groupUser_GroupId']}
                }
            },
            {$sort:{SentTime:-1}},
            {$limit:1}
        ],
        as:'stockdata2'
    }
}
";
            var set = "{$set:{stockdata2:{$arrayElemAt:['$stockdata2',0]}}}";
            var match = GetMatch(userId, groupIds);
            string project = "{$project:{_id:0,GroupId:1,LastSentTime:'$stockdata2.SentTime',Count:{$size:'$stockdata'}}}";
            string sort = "{$sort:{Count:-1,LastSentTime:-1}}";
            string skip = ""; string limit = "";
            if (pageSettings is not null)
            {
                skip = $"{{$skip:{(pageSettings.PageNum - 1) * pageSettings.PageSize}}}";
                limit = $"{{$limit:{pageSettings.PageSize}}}";
            }
            stages.Add(match);
            stages.Add(lookup1);
            stages.Add(lookup2);
            stages.Add(set);
            stages.Add(project);
            stages.Add(sort);
            if (pageSettings is not null)
            {
                stages.Add(skip);
                stages.Add(limit);
            }

            var result = await _repository.GetList<GroupUser,MessageCountGroupByGroupId>(stages, cancellationToken).ConfigureAwait(false);
            return result;
        }

        public async Task<Domain.Message> GetLastMessageBygGroupIdAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return await _repository.Query<Domain.Message>().Where(x => x.GroupId == groupId).OrderByDescending(x => x.SentTime).FirstOrDefaultAsync(cancellationToken);
        }

        private string GetLookup(string userId)
        {
            string lookup = $@"
{{
    $lookup:{{
        from:'Message',
        let:{{groupUser_GroupId:'$GroupId',groupUser_LastReadTime:'$LastReadTime'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$groupUser_GroupId']}},
                            {{$gt:['$SentTime','$$groupUser_LastReadTime']}},
                            {{$ne:['$SentBy','{userId}']}}
                        ]
                    }}
                }}
            }},
        ],
        as:'stockdata'
    }}
}}
";
            return lookup;
        }

        private string GetMatch(string userId, IEnumerable<string> groupIds)
        {
            var groupIdsStr = string.Join(",", groupIds.Select(x => $"'{x}'"));
            string match = $@"
{{$match:{{
    $and:[
        {{GroupId:{{$in:[{groupIdsStr}]}}}},
        {{UserId:'{userId}'}}
    ]
}}}}
";
            return match;
        }

        public async Task<IEnumerable<Domain.Message>> GetLastMessageForGroupsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            List<string> stages = new List<string>();
            var groupIdsStr = string.Join(",", groupIds.Select(x => $"'{x}'"));
            string match = $@"
{{$match:{{
    _id:{{$in:[{groupIdsStr}]}}
}}}}
";
            var lookup = $@"
{{
    $lookup:{{
        from:'Message',
        let:{{group_GroupId:'$_id'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$eq:['$GroupId','$$group_GroupId']}}
                }}
            }},
            {{$sort:{{SentTime:-1}}}},
            {{$limit:1}}

        ],
        as:'stockdata'
    }}
}}
";
            string set = "{$set:{stockdata:{$arrayElemAt:['$stockdata',0]}}}";
            string project = "{$project:{_id:0,stockdata:'$stockdata'}}";
            string replaceRoot = "{$replaceRoot:{newRoot:{$mergeObjects:'$stockdata'}}}";
            stages.Add(match);
            stages.Add(lookup);
            stages.Add(set);
            stages.Add(project);
            stages.Add(replaceRoot);
            var result = await _repository.GetList<Group, Domain.Message>(stages, cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}