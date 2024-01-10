using SugarChat.Core.IRepositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Dtos.Conversations;
using SugarChat.Message.Paging;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Dtos;
using SugarChat.Core.Utils;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Diagnostics;
using Serilog;

namespace SugarChat.Core.Services.Conversations
{
    public class ConversationDataProvider : IConversationDataProvider
    {
        private readonly IRepository _repository;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly ITableUtil _tableUtil;
        private readonly IMapper _mapper;

        public ConversationDataProvider(IRepository repository, IGroupDataProvider groupDataProvider, ITableUtil tableUtil, IMapper mapper)
        {
            _repository = repository;
            _groupDataProvider = groupDataProvider;
            _tableUtil = tableUtil;
            _mapper = mapper;
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

        public async Task<PagedResult<ConversationDto>> GetConversationListAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            IEnumerable<SearchParamDto> searchParams,
            IEnumerable<SearchMessageParamDto> searchByKeywordParams,
            int pageNum,
            int pageSize,
            int monthsAgo,
            CancellationToken cancellationToken = default)
        {
            var groupUsers = new List<GroupUser>();
            var total = 0;

            if (searchByKeywordParams != null && searchByKeywordParams.Any())
            {
                var whereByMessage = _tableUtil.GetWhereByMessage(filterGroupIds, searchByKeywordParams);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var queryByMessage = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<Domain.Message>(), whereByMessage);
                if (monthsAgo > 0)
                {
                    var startTime = DateTime.Now.AddMonths(-monthsAgo);
                    queryByMessage = queryByMessage.Where(x => x.SentTime > startTime);
                }
                var groupIdsByMessage = queryByMessage
                    .GroupBy(x => x.GroupId)
                    .Select(x => x.Key)
                    .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByMessage run {@Ms}{@Where}{@Total}", stopwatch.ElapsedMilliseconds, whereByMessage, groupIdsByMessage.Count());

                var whereByGroupUser = _tableUtil.GetWhereByGroupUser(userId, filterGroupIds, groupType, searchParams);
                stopwatch.Restart();
                var groupIdsByGroupUser = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUser>(), whereByGroupUser)
                    .OrderByDescending(x => x.UnreadCount)
                    .ThenByDescending(x => x.LastSentTime)
                    .Select(x => x.GroupId)
                    .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByGroupUser run {@Ms}{@Where}{@Total}", stopwatch.ElapsedMilliseconds, whereByGroupUser, groupIdsByGroupUser.Count());

                var temepGroups = new List<TemepGroup>();
                stopwatch.Restart();
                for (int i = 0; i < groupIdsByGroupUser.Count(); i++)
                {
                    temepGroups.Add(new TemepGroup { GroupId = groupIdsByGroupUser[i], Sort = i });
                }
                var groupIds = temepGroups.Where(x => groupIdsByMessage.Contains(x.GroupId)).OrderBy(x => x.Sort)
                    .Select(x => x.GroupId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByGroupUserAndMessage run {@Ms}{@GroupUserWhere}{@MessageWhere}", stopwatch.ElapsedMilliseconds, whereByGroupUser, whereByMessage);

                groupUsers = _repository.Query<GroupUser>().Where(x => x.UserId == userId && groupIds.Contains(x.GroupId))
                    .OrderByDescending(x => x.UnreadCount)
                    .ThenByDescending(x => x.LastSentTime)
                    .ToList();
                total = temepGroups.Where(x => groupIds.Contains(x.GroupId)).Count();
            }
            else
            {
                var where = _tableUtil.GetWhereByGroupUser(userId, filterGroupIds, groupType, searchParams);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                groupUsers = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUser>(), where)
                   .OrderByDescending(x => x.UnreadCount)
                   .ThenByDescending(x => x.LastSentTime)
                   .Skip((pageNum - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByGroupUser run {@Ms}{@Where}", stopwatch.ElapsedMilliseconds, where);
                stopwatch.Restart();
                total = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUser>(), where).Count();
                stopwatch.Stop();
                Log.Information("GetTotalByGroupUser run {@Ms}{@Where}", stopwatch.ElapsedMilliseconds, where);
            }

            var conversationDtos = await GetConversationListByGroupUsersAsync(groupUsers, cancellationToken).ConfigureAwait(false);

            return new PagedResult<ConversationDto>
            {
                Result = conversationDtos,
                Total = total
            };
        }

        class TemepGroup
        {
            public string GroupId { get; set; }
            public int Sort { get; set; }
        }

        private async Task<List<ConversationDto>> GetConversationListByGroupUsersAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken)
        {
            if (groupUsers == null || !groupUsers.Any())
                return new List<ConversationDto>();

            var groupIds = groupUsers.Select(x => x.GroupId).ToList();
            var groups = _repository.Query<Group>().Where(x => groupIds.Contains(x.Id)).ToList();
            var messageIds = new List<string>();
            if (groups.All(x => !string.IsNullOrWhiteSpace(x.LastMessageId)))
            {
                messageIds = groups.Select(x => x.LastMessageId).ToList();
            }
            else
            {
                messageIds = (from a in _repository.Query<Domain.Message>()
                              where groupIds.Contains(a.GroupId)
                              group a by a.GroupId into b
                              orderby b.Max(x => x.SentTime) descending
                              select new { b.First().Id })
                              .ToList()
                              .Select(x => x.Id)
                              .ToList();
            }
            var messages = await _repository.ToListAsync<Domain.Message>(x => messageIds.Contains(x.Id), cancellationToken).ConfigureAwait(false);

            var groupDtos = _mapper.Map<List<GroupDto>>(groups);
            var conversationDtos = new List<ConversationDto>();
            foreach (var groupUser in groupUsers)
            {
                var message = messages.SingleOrDefault(x => x.GroupId == groupUser.GroupId);
                var group = groups.Single(x => x.Id == groupUser.GroupId);
                ConversationDto conversationDto = new ConversationDto
                {
                    ConversationID = groupUser.GroupId,
                    UnreadCount = groupUser.UnreadCount,
                    LastMessageSentTime = groupUser.LastSentTime,
                    LastMessage = _mapper.Map<MessageDto>(message),
                    GroupProfile = _mapper.Map<GroupDto>(group),
                };
                conversationDtos.Add(conversationDto);
            }
            return conversationDtos;
        }

        public async Task<PagedResult<ConversationDto>> GetUnreadConversationListAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            IEnumerable<SearchParamDto> searchParams,
            IEnumerable<SearchMessageParamDto> searchByKeywordParams,
            int pageNum,
            int pageSize,
            int monthsAgo,
            CancellationToken cancellationToken = default)
        {
            var groupUsers = new List<GroupUser>();
            var total = 0;

            if (searchByKeywordParams != null && searchByKeywordParams.Any())
            {
                var whereByMessage = _tableUtil.GetWhereByMessage(filterGroupIds, searchByKeywordParams);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var queryByMessage = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<Domain.Message>(), whereByMessage);
                if (monthsAgo > 0)
                {
                    var startTime = DateTime.Now.AddMonths(-monthsAgo);
                    queryByMessage = queryByMessage.Where(x => x.SentTime > startTime);
                }
                var groupIdsByMessage = queryByMessage
                    .GroupBy(x => x.GroupId)
                    .Select(x => x.Key)
                    .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByMessage run {@Ms}{@Where}{@Total}", stopwatch.ElapsedMilliseconds, whereByMessage, groupIdsByMessage.Count());

                var whereByGroupUser = _tableUtil.GetWhereByGroupUser(userId, filterGroupIds, groupType, searchParams);
                stopwatch.Restart();
                whereByGroupUser = @"UnreadCount>0 and " + whereByGroupUser;
                var groupIdsByGroupUser = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUser>(), whereByGroupUser)
                    .OrderByDescending(x => x.UnreadCount)
                    .ThenByDescending(x => x.LastSentTime)
                    .Select(x => x.GroupId)
                    .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByGroupUser run {@Ms}{@Where}{@Total}", stopwatch.ElapsedMilliseconds, whereByGroupUser, groupIdsByGroupUser.Count());

                var temepGroups = new List<TemepGroup>();
                stopwatch.Restart();
                for (int i = 0; i < groupIdsByGroupUser.Count(); i++)
                {
                    temepGroups.Add(new TemepGroup { GroupId = groupIdsByGroupUser[i], Sort = i });
                }
                var groupIds = temepGroups.Where(x => groupIdsByMessage.Contains(x.GroupId)).OrderBy(x => x.Sort)
                    .Select(x => x.GroupId)
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByGroupUserAndMessage run {@Ms}{@GroupUserWhere}{@MessageWhere}", stopwatch.ElapsedMilliseconds, whereByGroupUser, whereByMessage);

                groupUsers = _repository.Query<GroupUser>().Where(x => x.UserId == userId && groupIds.Contains(x.GroupId))
                    .OrderByDescending(x => x.UnreadCount)
                    .ThenByDescending(x => x.LastSentTime)
                    .ToList();
                total = temepGroups.Where(x => groupIdsByGroupUser.Contains(x.GroupId)).Count();
            }
            else
            {
                var where = _tableUtil.GetWhereByGroupUser(userId, filterGroupIds, groupType, searchParams);
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                where = @"UnreadCount>0 and " + where;
                groupUsers = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUser>(), where)
                   .OrderByDescending(x => x.UnreadCount)
                   .ThenByDescending(x => x.LastSentTime)
                   .Skip((pageNum - 1) * pageSize)
                   .Take(pageSize)
                   .ToList();
                stopwatch.Stop();
                Log.Information("GetGroupIdsByGroupUser run {@Ms}{@Where}", stopwatch.ElapsedMilliseconds, where);
                stopwatch.Restart();
                total = System.Linq.Dynamic.Core.DynamicQueryableExtensions.Where(_repository.Query<GroupUser>(), where).Sum(x => x.UnreadCount);
                stopwatch.Stop();
                Log.Information("GetTotalByGroupUser run {@Ms}{@Where}", stopwatch.ElapsedMilliseconds, where);
            }

            var conversationDtos = await GetConversationListByGroupUsersAsync(groupUsers, cancellationToken).ConfigureAwait(false);

            return new PagedResult<ConversationDto>
            {
                Result = conversationDtos,
                Total = total
            };
        }
    }
}