using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Paging;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using SugarChat.Message.Dtos;
using System.Diagnostics;
using Serilog;
using SugarChat.Core.Utils;

namespace SugarChat.Core.Services.Groups
{
    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IRepository _repository;
        private readonly ITableUtil _tableUtil;

        public GroupDataProvider(IRepository repository, ITableUtil tableUtil)
        {
            _repository = repository;
            _tableUtil = tableUtil;
        }

        public async Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Group>(x => x.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Message.Paging.PagedResult<Group>> GetByIdsAsync(IEnumerable<string> ids, PageSettings pageSettings,
            CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Group>().Where(o => ids.Contains(o.Id))
                .OrderByDescending(o => o.LastModifyDate);
            var result = new Message.Paging.PagedResult<Group>();
            if (pageSettings != null)
            {
                result = await _repository.ToPagedListAsync(pageSettings, query, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var groups = query.ToList();
                result.Result = groups;
                result.Total = groups.Count();
            }
            return result;
        }

        public async Task AddAsync(Group group, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.AddAsync(group, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.AddGroupFailed.WithParams(group.Id));
            }
        }

        public async Task UpdateAsync(Group group, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.UpdateAsync(group, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.UpdateGroupFailed.WithParams(group.Id));
            }
        }

        public async Task RemoveAsync(Group group, CancellationToken cancellationToken = default)
        {
            int affectedLineNum = await _repository.RemoveAsync(group, cancellationToken).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(Prompt.RemoveGroupFailed.WithParams(group.Id));
            }
        }

        public async Task<IEnumerable<Group>> GetByCustomPropertiesAsync(IEnumerable<string> groupIds, Dictionary<string, string> customProperties, PageSettings pageSettings, CancellationToken cancellationToken = default)
        {
            groupIds = groupIds ?? new List<string>();
            if (customProperties != null && customProperties.Any())
            {
                var query = _repository.Query<GroupCustomProperty>();
                if (groupIds.Any())
                {
                    query = query.Where(x => groupIds.Contains(x.GroupId));
                }
                var sb = new StringBuilder();
                foreach (var customProperty in customProperties)
                {
                    var values = customProperty.Value.Split(',');
                    foreach (var value in values)
                    {
                        var _value = value.Replace("\\", "\\\\");
                        var _sb = $"{nameof(GroupCustomProperty.Key)}==\"{customProperty.Key}\" && {nameof(GroupCustomProperty.Value)} == \"{_value}\"";
                        sb.Append($" || ({_sb})");
                    }
                }
                query = query.Where(sb.ToString().Substring(4));
                var groupCustomProperties = await _repository.ToListAsync(query, cancellationToken).ConfigureAwait(false);
                var groupCustomPropertyGroups = groupCustomProperties.GroupBy(x => x.GroupId);
                var _groupIds = new List<string>();
                foreach (var groupCustomPropertyGroup in groupCustomPropertyGroups)
                {
                    if (groupCustomPropertyGroup.Count() == customProperties.Count())
                    {
                        _groupIds.Add(groupCustomPropertyGroup.Key);
                    }
                }
                groupIds = _groupIds;
            }
            if (pageSettings != null)
            {
                return (await _repository.ToPagedListAsync<Group>(pageSettings, x => groupIds.Contains(x.Id), cancellationToken).ConfigureAwait(false)).Result;
            }
            else
            {
                return await _repository.ToListAsync<Group>(x => groupIds.Contains(x.Id));
            }
        }

        public async Task<int> GetCountAsync(Expression<Func<Group, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return await _repository.CountAsync(predicate, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Group>> GetListAsync(PageSettings pageSettings, Expression<Func<Group, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return (await _repository.ToPagedListAsync(pageSettings, predicate, cancellationToken).ConfigureAwait(false)).Result;
        }

        public async Task UpdateRangeAsync(IEnumerable<Group> groups, CancellationToken cancellationToken = default)
        {
            await _repository.UpdateRangeAsync(groups, cancellationToken).ConfigureAwait(false);
        }

        public IEnumerable<string> GetGroupIds(Expression<Func<Group, bool>> predicate = null)
        {
            return _repository.Query<Group>().Where(predicate).Select(x => x.Id).ToList();
        }

        public async Task<(IEnumerable<string>, int)> GetGroupIdsAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            PageSettings pageSettings,
            Dictionary<string, string> searchParms, bool isExactSearch,
            SearchGroupByGroupCustomPropertiesDto includeGroupByGroupCustomProperties,
            SearchGroupByGroupCustomPropertiesDto excludeGroupByGroupCustomProperties,
            bool onlyUnread,
            CancellationToken cancellationToken = default)
        {
            var includeSb = _tableUtil.GetWhereByGroupCustomPropery(includeGroupByGroupCustomProperties, "GroupKey", "GroupValue");
            var searchSb = _tableUtil.GetWhereByMessage(searchParms, isExactSearch, "MessageKey", "MessageValue");
            var sb = includeSb + searchSb;

            var query = _tableUtil.GetQuery(userId,
                    filterGroupIds,
                    groupType,
                    includeSb.Length > 0,
                    searchSb.Length > 0);

            if (onlyUnread)
            {
                query = query.Where(x => x.UnreadCount > 0);
            }

            if (sb.Length > 0)
                query = query.Where(sb.ToString().Substring(4));

            var excludeSb = _tableUtil.GetWhereByGroupCustomPropery(excludeGroupByGroupCustomProperties);
            if (excludeSb.Length > 0)
            {
                var groupCustomProperties = await _repository.ToListAsync(_repository.Query<GroupCustomProperty>().Where(excludeSb.ToString().Substring(4)), cancellationToken).ConfigureAwait(false);
                var excludeGroupIds = groupCustomProperties.Select(x => x.GroupId).ToList();
                query = query.Where(x => !excludeGroupIds.Contains(x.GroupId));
            }
            query = query.Take(500);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            var groupIds = query.GroupBy(x => x.GroupId)
                    .OrderByDescending(x => x.Max(y => y.UnreadCount))
                    .ThenByDescending(x => x.Max(y => y.LastSentTime))
                    .Skip((pageSettings.PageNum - 1) * pageSettings.PageSize)
                    .Take(pageSettings.PageSize)
                    .Select(x => x.Key)
                    .ToList();
            stopwatch.Stop();
            Log.Information("GroupDataProvider.GetGroupIdsAsync1 run {@Ms}", stopwatch.ElapsedMilliseconds);

            stopwatch.Restart();
            var total = query.GroupBy(x => x.GroupId).Count();
            stopwatch.Stop();
            Log.Information("GroupDataProvider.GetGroupIdsAsync2 run {@Ms}, {@Total}", stopwatch.ElapsedMilliseconds, total);

            return (groupIds, total);
        }
    }
}