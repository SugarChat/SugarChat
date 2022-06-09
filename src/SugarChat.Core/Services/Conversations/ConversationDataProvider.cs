using System;
using SugarChat.Core.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using SugarChat.Core.Domain;
using System.Text;
using SugarChat.Core.Services.Groups;

namespace SugarChat.Core.Services.Conversations
{
    public class ConversationDataProvider : IConversationDataProvider
    {
        private readonly IRepository _repository;
        private readonly IGroupDataProvider _groupDataProvider;

        public ConversationDataProvider(IRepository repository, IGroupDataProvider groupDataProvider)
        {
            _repository = repository;
            _groupDataProvider = groupDataProvider;
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



        public async Task<List<ConversationDto>> GetConversationsByGroupKeywordAsync(string userId, Dictionary<string, string> searchParms, CancellationToken cancellationToken = default)
        {
            if (searchParms == null || !searchParms.Any())
            {
                return new List<ConversationDto>();
            }

            var conversations = new List<ConversationDto>();
            var groupIds = (await _groupDataProvider.GetByCustomProperties(null, searchParms, null, cancellationToken)).Select(x => x.Id);
            var groupUsers = await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId) && x.UserId == userId);
            var messageGroups = (from a in groupUsers
                                 join b in _repository.Query<Domain.Message>() on a.GroupId equals b.GroupId
                                 where b.SentTime > a.LastReadTime
                                 group b by b.GroupId into c
                                 select new
                                 {
                                     GroupId = c.Key,
                                     UnReadCount = c.Count(),
                                     LastMessageSentTime = c.Max(x => x.SentTime)
                                 }).ToList();
            foreach (var groupUser in groupUsers)
            {
                var conversationDto = new ConversationDto
                {
                    ConversationID = groupUser.GroupId
                };
                var messageGroup = messageGroups.FirstOrDefault(x => x.GroupId == groupUser.GroupId);
                if (messageGroup != null)
                {
                    conversationDto.LastMessageSentTime = messageGroup.LastMessageSentTime;
                    conversationDto.UnreadCount = messageGroup.UnReadCount;
                }
                conversations.Add(conversationDto);
            }
            return conversations;

            //            List<string> stages = new List<string>();
            //            var groupParms = new List<string>();
            //            foreach (var searchParm in searchParms)
            //            {
            //                var values = searchParm.Value.Split(',').Select(x => $"'{x}'");
            //                groupParms.Add($"{{'CustomProperties.{searchParm.Key}':{{$in:[{string.Join(",", values)}]}}}}");
            //            }
            //            var group = $@"
            //{{$match:
            //    {{$and:
            //        [
            //            {string.Join(',', groupParms)}
            //        ]
            //    }}
            //}}
            //";

            //            var groupUser = @$"
            //{{
            //    $lookup:{{
            //        from:'GroupUser',
            //        let:{{groupId:'$_id'}},
            //        pipeline:[
            //            {{$match:
            //                {{$expr:
            //                    {{$and:
            //                        [
            //                            {{$eq:['$GroupId','$$groupId']}},
            //                            {{$eq:['$UserId','{userId}']}}
            //                        ]
            //                    }}
            //                }}
            //            }}
            //        ],
            //        as:'GroupUser'
            //    }}
            //}}
            //";

            //            var groupUserShow = "{$project: {_id:0,GroupId:'$_id',size_of_groupUser: {$size:'$GroupUser'},GroupUser:1}}";

            //            var groupUserFilter = "{$match:{'size_of_groupUser':{$gt:0}}}";

            //            var groupUserSet = "{$set:{GroupUser:{$arrayElemAt:['$GroupUser',0]}}}";

            //            var unReadCount = @$"
            //{{
            //    $lookup:{{
            //        from:'Message',
            //        let:{{groupId:'$GroupId',lastReadTime:'$GroupUser.LastReadTime'}},
            //        pipeline:[
            //            {{$match:
            //                {{$expr:
            //                    {{$and:
            //                        [
            //                            {{$eq:['$GroupId','$$groupId']}},
            //                            {{$gt:['$SentTime','$$lastReadTime']}},
            //                            {{$ne:['$SentBy','{userId}']}}
            //                        ]
            //                    }}
            //                }}
            //            }},
            //        ],
            //        as:'Message1'
            //    }}
            //}}
            //";

            //            var lastReadTime = @"
            //{
            //    $lookup:{
            //        from:'Message',
            //        let:{groupId:'$GroupId'},
            //        pipeline:[
            //            {$match:
            //                {$expr:
            //                    {$and:
            //                        [
            //                            {$eq:['$GroupId','$$groupId']}
            //                        ]
            //                    }
            //                }
            //            },
            //            {$sort:{SentTime:1}}
            //        ],
            //        as:'Message2'
            //    }
            //}
            //";

            //            var set = "{$set:{Message2:{$arrayElemAt:['$Message2',0]}}}";
            //            var project = "{$project:{_id:0,ConversationID:'$GroupId',UnreadCount:{$size:'$Message1'},LastMessageSentTime:'$Message2.SentTime'}}";
            //            var sort = "{$sort:{UnreadCount:-1,LastMessageSentTime:-1}}";
            //            var limit = "{$limit:100}";

            //            stages.Add(group);
            //            stages.Add(groupUser);
            //            stages.Add(groupUserShow);
            //            stages.Add(groupUserFilter);
            //            stages.Add(groupUserSet);
            //            stages.Add(unReadCount);
            //            stages.Add(lastReadTime);
            //            stages.Add(set);
            //            stages.Add(project);
            //            stages.Add(sort);
            //            stages.Add(limit);
            //            var result = await _repository.GetList<Group, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            //            return result.ToList();
        }

        public async Task<List<ConversationDto>> GetConversationsByMessageKeywordAsync(string userId, Dictionary<string, string> searchParms, bool isExactSearch, CancellationToken cancellationToken = default)
        {
            if (searchParms == null || !searchParms.Any())
            {
                return new List<ConversationDto>();
            }
            var conversations = new List<ConversationDto>();
            var groupIds = await _groupDataProvider.GetGroupIdsByMessageKeywordAsync(null, searchParms, isExactSearch, cancellationToken);
            var groupUsers = await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId) && x.UserId == userId);
            var messageGroups = (from a in groupUsers
                                 join b in _repository.Query<Domain.Message>() on a.GroupId equals b.GroupId
                                 where b.SentTime > a.LastReadTime
                                 group b by b.GroupId into c
                                 select new
                                 {
                                     GroupId = c.Key,
                                     UnReadCount = c.Count(),
                                     LastMessageSentTime = c.Max(x => x.SentTime)
                                 }).ToList();
            foreach (var groupUser in groupUsers)
            {
                var conversationDto = new ConversationDto
                {
                    ConversationID = groupUser.GroupId
                };
                var messageGroup = messageGroups.FirstOrDefault(x => x.GroupId == groupUser.GroupId);
                if (messageGroup != null)
                {
                    conversationDto.LastMessageSentTime = messageGroup.LastMessageSentTime;
                    conversationDto.UnreadCount = messageGroup.UnReadCount;
                }
                conversations.Add(conversationDto);
            }
            return conversations;
        }

        public async Task<List<ConversationDto>> GetConversationsByUserAsync(string userId, PageSettings pageSettings, CancellationToken cancellationToken = default)
        {
            var groupUser = $@"
{{$match:{{
    $and:[
        {{UserId:{{$in:['{userId}']}}}}
    ]
}}}}
";

            var unReadCount = @$"
{{
    $lookup:{{
        from:'Message',
        let:{{groupId:'$GroupId',lastReadTime:'$LastReadTime'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$groupId']}},
                            {{$gt:['$SentTime','$$lastReadTime']}},
                            {{$ne:['$SentBy','{userId}']}}
                        ]
                    }}
                }}
            }},
        ],
        as:'Message1'
    }}
}}
";

            var lastReadTime = @"
{
    $lookup:{
        from:'Message',
        let:{groupId:'$GroupId'},
        pipeline:[
            {$match:
                {$expr:
                    {$and:
                        [
                            {$eq:['$GroupId','$$groupId']}
                        ]
                    }
                }
            },
            {$sort:{SentTime:1}}
        ],
        as:'Message2'
    }
}
";
            var set = "{$set:{Message2:{$arrayElemAt:['$Message2',0]}}}";
            var show = "{$project:{_id:0,ConversationID:'$GroupId',UnreadCount:{$size:'$Message1'},LastMessageSentTime:'$Message2.SentTime'}}";
            var sort = "{$sort:{UnreadCount:-1,LastMessageSentTime:-1}}";
            var skip = $"{{$skip:{(pageSettings.PageNum - 1) * pageSettings.PageSize}}}";
            var limit = $"{{$limit:{pageSettings.PageSize}}}";
            List<string> stages = new List<string>();
            stages.Add(groupUser);
            stages.Add(unReadCount);
            stages.Add(lastReadTime);
            stages.Add(set);
            stages.Add(show);
            stages.Add(sort);
            stages.Add(skip);
            stages.Add(limit);

            var result = await _repository.GetList<GroupUser, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            return result.ToList(); ;
        }
    }
}