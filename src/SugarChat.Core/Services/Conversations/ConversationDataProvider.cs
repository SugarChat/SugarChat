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

            List<string> stages = new List<string>();
            var groupParms = new List<string>();
            foreach (var searchParm in searchParms)
            {
                var values = searchParm.Value.Split(',').Select(x => $"'{x}'");
                groupParms.Add($"{{'CustomProperties.{searchParm.Key}':{{$in:[{string.Join(",", values)}]}}}}");
            }
            var group = $@"
{{$match:
    {{$and:
        [
            {string.Join(',', groupParms)}
        ]
    }}
}}
";

            var groupUser = @$"
{{
    $lookup:{{
        from:'GroupUser',
        let:{{groupId:'$_id'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$groupId']}},
                            {{$eq:['$UserId','{userId}']}}
                        ]
                    }}
                }}
            }}
        ],
        as:'GroupUser'
    }}
}}
";

            var groupUserShow = "{$project: {_id:0,GroupId:'$_id',size_of_groupUser: {$size:'$GroupUser'},GroupUser:1}}";

            var groupUserFilter = "{$match:{'size_of_groupUser':{$gt:0}}}";

            var groupUserSet = "{$set:{GroupUser:{$arrayElemAt:['$GroupUser',0]}}}";

            var unReadCount = @$"
{{
    $lookup:{{
        from:'Message',
        let:{{groupId:'$GroupId',lastReadTime:'$GroupUser.LastReadTime'}},
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
            var project = "{$project:{_id:0,ConversationID:'$GroupId',UnreadCount:{$size:'$Message1'},LastMessageSentTime:'$Message2.SentTime'}}";
            var sort = "{$sort:{UnreadCount:-1,LastMessageSentTime:-1}}";
            var limit = "{$limit:100}";

            stages.Add(group);
            stages.Add(groupUser);
            stages.Add(groupUserShow);
            stages.Add(groupUserFilter);
            stages.Add(groupUserSet);
            stages.Add(unReadCount);
            stages.Add(lastReadTime);
            stages.Add(set);
            stages.Add(project);
            stages.Add(sort);
            stages.Add(limit);
            var result = await _repository.GetList<Group, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            return result.ToList();
        }

        public async Task<List<ConversationDto>> GetConversationsByMessageKeywordAsync(string userId,
            Dictionary<string, string> searchGroupParms,
            Dictionary<string, string> searchMessageParms,
            bool isExactSearch,
            CancellationToken cancellationToken = default)
        {
            if (searchMessageParms == null || !searchMessageParms.Any())
            {
                return new List<ConversationDto>();
            }
            var conversations = new List<ConversationDto>();
            var groupIds1 = await _groupDataProvider.GetGroupIdsByMessageKeywordAsync(null, searchMessageParms, isExactSearch, cancellationToken);
            var groupIds2 = (await _groupDataProvider.GetByCustomProperties(null, searchGroupParms, null, cancellationToken)).Select(x => x.Id);
            var groupIds = groupIds1.Union(groupIds2);
            var groupUsers = await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId) && x.UserId == userId);
            var messageGroups = from a in _repository.Query<Domain.Message>()
                      join b in groupUsers on a.GroupId equals b.GroupId
                      where a.SentTime > b.LastReadTime
                      group a by a.GroupId into c
                      select new
                      {
                          GroupId = c.Key,
                          UnReadCount = c.Count(),
                          LastMessageSentTime = c.Max(x => x.SentTime)
                      };
            foreach (var groupUser in groupUsers)
            {
                var messageGroup = messageGroups.FirstOrDefault(x => x.GroupId == groupUser.GroupId);
                conversations.Add(new ConversationDto
                {
                    ConversationID = groupUser.GroupId,
                    LastMessageSentTime = messageGroup.LastMessageSentTime,
                    UnreadCount = messageGroup.UnReadCount
                });
            }
            return conversations;
            //var groupUsers=await _repository.ToListAsync()
            //            var messageFilter = @"
            //{$match:
            //    {$and:[
            //        #match_and_or#
            //    ]}
            //}
            //";

            //            if (searchMessageParms is not null && searchMessageParms.Any())
            //            {
            //                StringBuilder match_and_or = new StringBuilder("{$or:[");

            //                foreach (var searchParm in searchMessageParms)
            //                {
            //                    string[] chars = new string[] { "^", "$", ".", "*", "?", "+", "|", "{", "}", "[", "]", "/" };
            //                    var keyword = searchParm.Value.Replace(@"\", @"\\");
            //                    foreach (var item in chars)
            //                    {
            //                        keyword = keyword.Replace(item, @"\" + item);
            //                    }
            //                    if (searchParm.Key == Message.Constant.Content)
            //                    {
            //                        match_and_or.Append($"{{Content:/{keyword}/i}}");
            //                    }
            //                    else
            //                    {
            //                        if (isExactSearch)
            //                        {
            //                            match_and_or.Append($"{{'CustomProperties.{searchParm.Key}':/^{keyword}$/i}},");
            //                        }
            //                        else
            //                        {
            //                            match_and_or.Append($"{{'CustomProperties.{searchParm.Key}':/{keyword}/i}},");
            //                        }
            //                    }
            //                }
            //                match_and_or.Append("]}");
            //                messageFilter = messageFilter.Replace("#match_and_or#", match_and_or.ToString());
            //            }

            //            string groupByGroupId = "{$group:{_id:'$GroupId'}}";

            //            var groupParms = new List<string>();
            //            if (searchGroupParms != null && searchGroupParms.Any())
            //            {
            //                foreach (var searchGroupParm in searchGroupParms)
            //                {
            //                    var values = searchGroupParm.Value.Split(',').Select(x => $"'{x}'");
            //                    groupParms.Add($"{{$in:['$CustomProperties.{searchGroupParm.Key}',[{string.Join(",", values)}]]}}");
            //                }
            //            }
            //            string group = $@"
            //{{
            //    $lookup:{{
            //        from:'Group',
            //        let:{{groupId:'$_id'}},
            //        pipeline:[
            //            {{$match:
            //                {{$expr:
            //                    {{$and:
            //                        [
            //                            {{$eq:['$_id','$$groupId']}},
            //                            {string.Join(',', groupParms)}
            //                        ]
            //                    }}
            //                }}
            //            }}
            //        ],
            //        as:'Group'
            //    }}
            //}}
            //";

            //            string groupShow = "{$project:{_id:0,GroupId:'$_id',size_of_group:{$size:'$Group'}}}";

            //            string groupFilter = "{$match:{'size_of_group':{$gt:0}}}";

            //            string groupUser = $@"
            //{{
            //    $lookup:{{
            //        from:'GroupUser',
            //        let:{{groupId:'$GroupId'}},
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

            //            var groupUserShow = "{$project:{GroupId:1,size_of_GroupUser:{$size:'$GroupUser'},GroupUser:1}}";

            //            var groupUserFilter = "{$match:{'size_of_GroupUser':{$gt:0}}}";

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
            //            var show = "{$project:{_id:0,ConversationID:'$GroupId',UnreadCount:{$size:'$Message1'},LastMessageSentTime:'$Message2.SentTime'}}";
            //            var sort = "{$sort:{UnreadCount:-1,LastMessageSentTime:-1}}";
            //            var limit = "{$limit:100}";
            //            List<string> stages = new List<string>();
            //            stages.Add(messageFilter);
            //            stages.Add(groupByGroupId);
            //            stages.Add(group);
            //            stages.Add(groupShow);
            //            stages.Add(groupFilter);
            //            stages.Add(groupUser);
            //            stages.Add(groupUserShow);
            //            stages.Add(groupUserFilter);
            //            stages.Add(groupUserSet);
            //            stages.Add(unReadCount);
            //            stages.Add(lastReadTime);
            //            stages.Add(set);
            //            stages.Add(show);
            //            stages.Add(sort);
            //            stages.Add(limit);

            //            var result = await _repository.GetList<Domain.Message, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            //            return result.ToList();
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