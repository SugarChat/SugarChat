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

namespace SugarChat.Core.Services.Groups
{
    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IRepository _repository;

        public GroupDataProvider(IRepository repository)
        {
            _repository = repository;
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
                if (customProperties.Values.Count() == 1 && customProperties.Values.First().Split(',').Count() == 1)
                {
                    query = query.Where(x => x.Key == customProperties.Keys.First() && x.Value == customProperties.Values.First());
                    var groupCustomProperties = await _repository.ToListAsync(query, cancellationToken).ConfigureAwait(false);
                    groupIds = groupCustomProperties.Select(x => x.GroupId).ToList();
                }
                else
                {
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

        public async Task<IEnumerable<string>> GetGroupIdByIncludeCustomPropertiesAsync(IEnumerable<string> groupIds, SearchGroupByGroupCustomPropertiesDto includeGroupByGroupCustomProperties, CancellationToken cancellationToken = default)
        {
            if (includeGroupByGroupCustomProperties == null || includeGroupByGroupCustomProperties.GroupCustomProperties == null || !includeGroupByGroupCustomProperties.GroupCustomProperties.Any())
            {
                return groupIds;
            }
            return await GetGroupIdByCustomPropertiesAsync(groupIds, includeGroupByGroupCustomProperties, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetGroupIdByExcludeCustomPropertiesAsync(IEnumerable<string> groupIds, SearchGroupByGroupCustomPropertiesDto excludeGroupByGroupCustomProperties, CancellationToken cancellationToken = default)
        {
            if (excludeGroupByGroupCustomProperties == null || excludeGroupByGroupCustomProperties.GroupCustomProperties == null || !excludeGroupByGroupCustomProperties.GroupCustomProperties.Any())
            {
                return new List<string>();
            }
            return await GetGroupIdByCustomPropertiesAsync(groupIds, excludeGroupByGroupCustomProperties, cancellationToken).ConfigureAwait(false);
        }

        private async Task<IEnumerable<string>> GetGroupIdByCustomPropertiesAsync(IEnumerable<string> groupIds, SearchGroupByGroupCustomPropertiesDto searchGroupByGroupCustomProperties, CancellationToken cancellationToken = default)
        {
            var isExactSearch = searchGroupByGroupCustomProperties.IsExactSearch;
            var searchCustomProperties = searchGroupByGroupCustomProperties.GroupCustomProperties;
            foreach (var dic in searchCustomProperties)
            {
                var sb = new StringBuilder();
                foreach (var value in dic.Value)
                {
                    var value1 = value.Replace("\\", "\\\\");
                    var key1 = dic.Key.Replace("\\", "\\\\");
                    if (value1.Contains(","))
                    {
                        var values = value1.Split(',');
                        foreach (var value2 in values)
                        {
                            if (isExactSearch)
                                sb.Append($" || (Key==\"{key1}\" && Value==\"{value2}\")");
                            else
                                sb.Append($" || (Key==\"{key1}\" && Value.Contains(\"{value2}\"))");
                        }
                    }
                    else
                    {
                        if (isExactSearch)
                            sb.Append($" || (Key==\"{key1}\" && Value==\"{value1}\")");
                        else
                            sb.Append($" || (Key==\"{key1}\" && Value.Contains(\"{value1}\"))");
                    }
                }
                var query = _repository.Query<GroupCustomProperty>();
                if (groupIds != null && groupIds.Any())
                {
                    query = query.Where(x => groupIds.Contains(x.GroupId));
                }
                var groupCustomProperties = await _repository.ToListAsync(query.Where(sb.ToString().Substring(4)), cancellationToken).ConfigureAwait(false);
                groupIds = groupCustomProperties.Select(x => x.GroupId).ToList();
            }
            return groupIds;
        }

        public async Task<IEnumerable<string>> GetGroupIdsByMessageKeywordAsync(IEnumerable<string> groupIds, Dictionary<string, string> searchParms, bool isExactSearch, int groupType, CancellationToken cancellationToken = default)
        {
            groupIds = groupIds ?? new List<string>();
            if (searchParms != null && searchParms.Any())
            {
                var messageIds = new List<string>();
                var contentSearchParms = searchParms.Where(x => x.Key == Message.Constant.Content);
                var customPropertySearchParms = searchParms.Where(x => x.Key != Message.Constant.Content);
                if (contentSearchParms.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var contentSearchParm in contentSearchParms)
                    {
                        var value = contentSearchParm.Value.Replace("\\", "\\\\");
                        if (isExactSearch)
                        {
                            sb.Append($" || {Message.Constant.Content}==\"{value}\"");
                        }
                        else
                        {
                            sb.Append($" || {Message.Constant.Content}.Contains(\"{value}\")");
                        }
                    }
                    var messages = await _repository.ToListAsync(_repository.Query<Domain.Message>().Where(sb.ToString().Substring(4)), cancellationToken).ConfigureAwait(false);
                    messageIds = messages.Select(x => x.Id).ToList();
                }
                if (customPropertySearchParms.Any())
                {
                    var sb = new StringBuilder();
                    foreach (var customPropertySearchParm in customPropertySearchParms)
                    {
                        var values = customPropertySearchParm.Value.Split(',');
                        foreach (var value in values)
                        {
                            var _value = customPropertySearchParm.Value.Replace("\\", "\\\\");
                            if (isExactSearch)
                            {
                                var _sb = $"{nameof(MessageCustomProperty.Key)}==\"{customPropertySearchParm.Key}\" && {nameof(MessageCustomProperty.Value)} == \"{_value}\"";
                                sb.Append($" || {_sb}");
                            }
                            else
                            {
                                var _sb = $"{nameof(MessageCustomProperty.Key)}.Contains(\"{customPropertySearchParm.Key}\") && {nameof(MessageCustomProperty.Value)}.Contains(\"{_value}\")";
                                sb.Append($" || {_sb}");
                            }
                        }
                    }
                    var messageCustomProperties = await _repository.ToListAsync(_repository.Query<MessageCustomProperty>().Where(sb.ToString().Substring(4)), cancellationToken).ConfigureAwait(false);
                    var messageCustomPropertyGroups = messageCustomProperties.GroupBy(x => x.MessageId);
                    var _messageIds = new List<string>();
                    foreach (var messageCustomPropertyGroup in messageCustomPropertyGroups)
                    {
                        if (messageCustomPropertyGroup.Count() == customPropertySearchParms.Count())
                        {
                            _messageIds.Add(messageCustomPropertyGroup.Key);
                        }
                    }
                    messageIds.AddRange(_messageIds);
                }
                var _groupIds = (await _repository.ToListAsync<Domain.Message>(x => messageIds.Contains(x.Id))).Select(x => x.GroupId);
                groupIds = groupIds.Intersect(_groupIds).ToList();
                return (await _repository.ToListAsync<Group>(x => groupIds.Contains(x.Id) && x.Type == groupType)).Select(x => x.Id);
            }
            else
            {
                return new List<string>();
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
    }
}