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

namespace SugarChat.Core.Services.Conversations
{
    public class ConversationDataProvider : IConversationDataProvider
    {
        private readonly IRepository _repository;

        public ConversationDataProvider(IRepository repository)
        {
            _repository = repository;
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
            if (searchParms.Count == 0)
            {
                return default;
            }

            List<string> stages = new List<string>();
            var groupParms = new List<string>();
            foreach (var searchParm in searchParms)
            {
                var values = string.Join(",", searchParm.Value.Select(x => $"'{x}'"));
                groupParms.Add($"{{'CustomProperties.{searchParm.Key}':{{$in:[values]}}}}");
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
        let:{{_Group_Id:'$_id'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$_Group_Id']}},
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

            var unReadCount = @$"
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
        as:'Message1'
    }}
}}
";

            var lastReadTime = @"
{
    $lookup:{
        from:'Message',
        let:{groupUser_GroupId:'$GroupId',groupUser_LastReadTime:'$LastReadTime'},
        pipeline:[
            {$match:
                {$expr:
                    {$and:
                        [
                            {$eq:['$GroupId','$$groupUser_GroupId']}
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
            var project = "{$project:{Count:{$size:'$Message1'},SeneTime:'$Message2.SentTime'}}";
            var sort = "{$sort:{Count:-1,SeneTime:-1}}";
            var limit = "{$limit:100}";

            stages.Add(group);
            stages.Add(groupUser);
            stages.Add(unReadCount);
            stages.Add(lastReadTime);
            stages.Add(set);
            stages.Add(project);
            stages.Add(sort);
            stages.Add(limit);
            var result = await _repository.GetList<Group, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            return result.ToList();
        }

        public async Task<List<ConversationDto>> GetConversationsByMessageKeywordAsync(string userId, Dictionary<string, string> searchParms, bool isExactSearch, CancellationToken cancellationToken = default)
        {
            if (searchParms.Count == 0)
            {
                return default;
            }

            var match = @"
{$match:
    {$and:[
        #match_and_or#
    ]}
}
";
            if (searchParms is not null && searchParms.Any())
            {
                StringBuilder match_and_or = new StringBuilder("{$or:[");

                foreach (var searchParm in searchParms)
                {
                    string[] chars = new string[] { "^", "$", ".", "*", "?", "+", "|", "{", "}", "[", "]", "/" };
                    var keyword = searchParm.Value.Replace(@"\", @"\\");
                    foreach (var item in chars)
                    {
                        keyword = keyword.Replace(item, @"\" + item);
                    }
                    if (searchParm.Key == Message.Constant.Content)
                    {
                        match_and_or.Append($"{{Content:/{keyword}/i}}");
                    }
                    else
                    {
                        if (isExactSearch)
                        {
                            match_and_or.Append($"{{'CustomProperties.{searchParm.Key}':/^{keyword}$/i}},");
                        }
                        else
                        {
                            match_and_or.Append($"{{'CustomProperties.{searchParm.Key}':/{keyword}/i}},");
                        }
                    }
                }
                match_and_or.Append("]}");
                match = match.Replace("#match_and_or#", match_and_or.ToString());
            }

            string groupByGroupId = "{$group:{_id:'$GroupId'}}";

            string groupUser = $@"
{{
    $lookup:{{
        from:'GroupUser',
        let:{{message_GroupId:'$_id'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$message_GroupId']}},
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
            var groupUserShow = "{$project:{size_of_GroupUser:{$size:'$GroupUser'}}}";

            var groupGroupFilter = "{$match:{'size_of_GroupUser':{$gt:0}}}";

            var unReadCount = @$"
{{
    $lookup:{{
        from:'Message',
        let:{{group_Id:'$_id',groupUser_LastReadTime:'$LastReadTime'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$group_Id']}},
                            {{$gt:['$SentTime','$$groupUser_LastReadTime']}},
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
        let:{group_Id:'$_id'},
        pipeline:[
            {$match:
                {$expr:
                    {$and:
                        [
                            {$eq:['$GroupId','$$group_Id']}
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
            var show = "{$project:{_id:0,ConversationID:'$_id',UnreadCount:{$size:'$Message1'},LastMessageSentTime:'$Message2.SentTime'}}";
            var sort = "{$sort:{UnreadCount:-1,LastMessageSentTime:-1}}";
            var limit = "{$limit:100}";
            List<string> stages = new List<string>();
            stages.Add(match);
            stages.Add(groupByGroupId);
            stages.Add(groupUser);
            stages.Add(groupUserShow);
            stages.Add(groupGroupFilter);
            stages.Add(unReadCount);
            stages.Add(lastReadTime);
            stages.Add(set);
            stages.Add(show);
            stages.Add(sort);
            stages.Add(limit);

            var result = await _repository.GetList<Domain.Message, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            return result.ToList(); ;
        }

        public async Task<List<ConversationDto>> GetConversationsByUser(string userId, PageSettings pageSettings, CancellationToken cancellationToken = default)
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
        let:{{group_Id:'$_id',groupUser_LastReadTime:'$LastReadTime'}},
        pipeline:[
            {{$match:
                {{$expr:
                    {{$and:
                        [
                            {{$eq:['$GroupId','$$group_Id']}},
                            {{$gt:['$SentTime','$$groupUser_LastReadTime']}},
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
        let:{group_Id:'$_id'},
        pipeline:[
            {$match:
                {$expr:
                    {$and:
                        [
                            {$eq:['$GroupId','$$group_Id']}
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
            var show = "{$project:{_id:0,ConversationID:'$_id',UnreadCount:{$size:'$Message1'},LastMessageSentTime:'$Message2.SentTime'}}";
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
            stages.Add(limit);

            var result = await _repository.GetList<GroupUser, ConversationDto>(stages, cancellationToken).ConfigureAwait(false);
            return result.ToList(); ;
        }
    }
}