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

            //var aaa = (from a in _repository.Query<GroupUser>()
            //           where groupIds.Contains(a.GroupId)
            //           join b in _repository.Query<Domain.Message>() on a.GroupId equals b.GroupId
            //           where b.SentTime > a.LastReadTime
            //           select a).ToList();

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
            var conversations = new List<ConversationDto>();
            var groupUsers = await _repository.ToListAsync<GroupUser>(x => x.UserId == userId);
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
    }
}