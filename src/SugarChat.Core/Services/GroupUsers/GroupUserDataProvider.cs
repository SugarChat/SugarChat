using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using System.Linq;
using System.Linq.Expressions;
using SugarChat.Message.Paging;
using System.Diagnostics;
using Serilog;
using System.Text;
using System.Linq.Dynamic.Core;
using SugarChat.Core.Utils;
using AutoMapper.Internal;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserDataProvider : IGroupUserDataProvider
    {
        private readonly IRepository _repository;

        public GroupUserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(GroupUser groupUser, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddAsync(groupUser, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.AddGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByUserIdAsync(string userId, IEnumerable<string> filterGroupIds, int groupType, CancellationToken cancellationToken = default)
        {
            var groupUsers = new List<GroupUser>();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (filterGroupIds != null && filterGroupIds.Any())
            {
                groupUsers = (from a in _repository.Query<GroupUser>()
                              join b in _repository.Query<Group>() on a.GroupId equals b.Id
                              where a.UserId == userId && b.Type == groupType && filterGroupIds.Contains(b.Id)
                              select new GroupUser
                              {
                                  Id = a.Id,
                                  UserId = a.UserId,
                                  GroupId = a.GroupId,
                                  Role = a.Role,
                                  MessageRemindType = a.MessageRemindType,
                                  UnreadCount = a.UnreadCount
                              }).ToList();
                stopwatch.Stop();
                Log.Information("GroupUserDataProvider.GetByUserIdAsync1 run {@Ms}, {@GroupIdTotal}, {@GroupUserTotal}", stopwatch.ElapsedMilliseconds, filterGroupIds.Count(), groupUsers.Count());
            }
            else
            {
                groupUsers = (from a in _repository.Query<GroupUser>()
                              join b in _repository.Query<Group>() on a.GroupId equals b.Id
                              where a.UserId == userId && b.Type == groupType
                              select new GroupUser
                              {
                                  Id = a.Id,
                                  UserId = a.UserId,
                                  GroupId = a.GroupId,
                                  Role = a.Role,
                                  MessageRemindType = a.MessageRemindType,
                                  UnreadCount = a.UnreadCount
                              }).ToList();
                stopwatch.Stop();
                Log.Information("GroupUserDataProvider.GetByUserIdAsync2 run {@Ms}, {@Total}", stopwatch.ElapsedMilliseconds, groupUsers.Count());
            }

            var result = new List<GroupUser>();

            stopwatch.Restart();
            foreach (var groupUser in groupUsers)
            {
                result.Add(new GroupUser
                {
                    Id = groupUser.Id,
                    UserId = groupUser.UserId,
                    GroupId = groupUser.GroupId,
                    Role = groupUser.Role,
                    MessageRemindType = groupUser.MessageRemindType
                });
            }
            stopwatch.Stop();
            Log.Information("GroupUserDataProvider.GetByUserIdAsync3 run {@Ms}", stopwatch.ElapsedMilliseconds);

            return result;
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdAsync(string id,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.GroupId == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<GroupUser> GetByUserAndGroupIdAsync(string userId, string groupId,
            CancellationToken cancellationToken = default)
        {
            return await _repository
                .SingleOrDefaultAsync<GroupUser>(o => o.UserId == userId && o.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task RemoveAsync(GroupUser groupUser, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveAsync(groupUser, cancellationToken);
        }

        public async Task UpdateAsync(GroupUser groupUser, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.UpdateAsync(groupUser, cancellationToken);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.UpdateGroupUserFailed.WithParams(groupUser.Id));
            }
        }

        public async Task UpdateAsync(IEnumerable<GroupUser> groupUsers, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateRangeAsync(groupUsers, cancellationToken);
        }

        public async Task<IEnumerable<GroupUser>> GetMembersByGroupIdAsync(string id,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => x.GroupId == id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetGroupMemberCountByGroupIdAsync(string groupId, CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync<GroupUser>(x => x.GroupId == groupId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task AddRangeAsync(IEnumerable<GroupUser> groupUsers,
            CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != groupUsers.Count())
            {
                throw new BusinessWarningException(Prompt.AddGroupUsersFailed.WithParams(groupUsers.Count().ToString(),
                    affectedLineNum.ToString()));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdAndUsersIdAsync(string groupId,
            IEnumerable<string> userIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(o => o.GroupId == groupId && userIds.Contains(o.UserId),
                cancellationToken);
        }

        public async Task UpdateRangeAsync(IEnumerable<GroupUser> groupUsers,
            CancellationToken cancellationToken = default)
        {
            await _repository.UpdateRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
        }

        public async Task UpdateRangeAsync(IEnumerable<GroupUser> source, IEnumerable<GroupUser> destination, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateRangeAsync(source, destination, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<GroupUser> groupUsers,
            CancellationToken cancellationToken = default)
        {
            int affectedLineNum =
                await _repository.RemoveRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != groupUsers.Count())
            {
                throw new BusinessWarningException(Prompt.RemoveGroupUsersFailed.WithParams(
                    groupUsers.Count().ToString(),
                    affectedLineNum.ToString()));
            }
        }

        public async Task<IEnumerable<GroupUser>> GetByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetGroupMemberCountByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => groupIds.Contains(x.GroupId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetListByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUser>(x => ids.Contains(x.Id), cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> GetCountAsync(Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUser>> GetListAsync(PageSettings pageSettings, Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return (await _repository.ToPagedListAsync(pageSettings, predicate, cancellationToken).ConfigureAwait(false)).Result;
        }

        public async Task<List<GroupUser>> GetListAsync(Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(Expression<Func<GroupUser, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> FilterGroupUserByCustomProperties(IEnumerable<string> groupUserIds, Dictionary<string, List<string>> customProperties, CancellationToken cancellationToken = default)
        {
            if (customProperties == null || !customProperties.Any())
            {
                return groupUserIds;
            }
            var sb = new StringBuilder();
            foreach (var dic in customProperties)
            {
                foreach (var value in dic.Value)
                {
                    var value1 = value.Replace("\\", "\\\\");
                    var key1 = dic.Key.Replace("\\", "\\\\");
                    if (value1.Contains(","))
                    {
                        var values = value1.Split(',');
                        foreach (var value2 in values)
                        {
                            sb.Append($" || (CustomProperties.\"{key1}\"==\"{value2}\")");
                        }
                    }
                    else
                        sb.Append($" || (CustomProperties.{key1}==\"{value1}\")");
                }
            }
            var query = _repository.Query<GroupUser>();
            if (groupUserIds != null && groupUserIds.Any())
            {
                query = query.Where(x => groupUserIds.Contains(x.Id));
            }
            var groupUsers = await _repository.ToListAsync(query.Where(sb.ToString().Substring(4)), cancellationToken).ConfigureAwait(false);
            return groupUsers.Select(x => x.Id).ToList();
        }

        public IEnumerable<string> FilterGroupUserByCustomProperties(IEnumerable<GroupUser> groupUsers, Dictionary<string, List<string>> customProperties)
        {
            if (customProperties == null || !customProperties.Any())
            {
                return new List<string>();
            }
            var predicate = PredicateBuilder.False<GroupUser>();
            var sb = new StringBuilder();
            foreach (var dic in customProperties)
            {
                foreach (var value in dic.Value)
                {
                    var value1 = value.Replace("\\", "\\\\");
                    var key1 = dic.Key.Replace("\\", "\\\\");
                    if (value1.Contains(","))
                    {
                        var values = value1.Split(',');
                        foreach (var value2 in values)
                        {
                            if (groupUsers.Select(x => x.CustomProperties).Any(x => x.GetOrDefault(key1) != null))
                                predicate = predicate.Or(x => x.CustomProperties[key1] == value2);
                        }
                    }
                    else
                    {
                        if (groupUsers.Select(x => x.CustomProperties).Any(x => x.GetOrDefault(key1) != null))
                            predicate = predicate.Or(x => x.CustomProperties[key1] == value1);
                    }
                }
            }
            groupUsers = groupUsers.AsQueryable().Where(predicate).ToList();
            return groupUsers.Select(x => x.Id).ToList();
        }
    }
}
