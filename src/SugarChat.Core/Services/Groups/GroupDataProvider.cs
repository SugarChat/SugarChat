using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Paging;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using SugarChat.Core.Extensions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

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

        public async Task<IEnumerable<Group>> GetByCustomProperties(List<string> groupIds, Dictionary<string, string> customProperties, PageSettings pageSettings, CancellationToken cancellationToken = default)
        {
            groupIds = groupIds ?? new List<string>();
            if (customProperties != null && customProperties.Any())
            {
                var query = _repository.Query<GroupCustomProperty>().Where(x => groupIds.Contains(x.GroupId));
                var sb1 = new StringBuilder(
                    $"{nameof(GroupCustomProperty.Key)}=\"{customProperties.ElementAt(0).Key}\" && {nameof(GroupCustomProperty.Value)} == \"{customProperties.ElementAt(0).Value}\"");
                for (int i = 1; i < customProperties.Count; i++)
                {
                    var index = i;
                    var sb2 = new StringBuilder($"{nameof(GroupCustomProperty.Key)}=\"{customProperties.ElementAt(index).Key}\" && {nameof(GroupCustomProperty.Value)} == \"{customProperties.ElementAt(index).Value}\"");
                    sb1.Append($" || {sb2}");
                }
                query = query.Where(sb1.ToString());
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
                if (groupIds.Any())
                {

                    groupIds = groupIds.Intersect(_groupIds).ToList();
                }
                else
                {
                    groupIds = _groupIds;
                }
            }
            else
            {
                return new List<Group>();
            }
            if (pageSettings != null)
            {
                return (await _repository.ToPagedListAsync<Group>(pageSettings, x => groupIds.Contains(x.Id), cancellationToken).ConfigureAwait(false)).Result;
            }
            else
            {
                return await _repository.ToListAsync<Group>(x => groupIds.Contains(x.Id));
            }

            //            var match = @"
            //{$match:
            //    {$and:[
            //        {_id:{$in:[#GroupIds#]}},
            //        #customProperties#
            //    ]}
            //}
            //";
            //            if (groupIds.Count() > 0)
            //            {
            //                var groupIdsStr = string.Join(",", groupIds.Select(x => $"'{x}'"));
            //                match = match.Replace("#GroupIds#", groupIdsStr);
            //            }
            //            else
            //            {
            //                match = match.Replace("{_id:{$in:[#GroupIds#]}},", "");
            //            }

            //            List<string> customPropertyQuery = new List<string>();
            //            if (customProperties != null && customProperties.Any())
            //            {
            //                foreach (var customProperty in customProperties)
            //                {
            //                    var values = customProperty.Value.Split(',');
            //                    values = values.Select(x => $"'{x}'").ToArray();
            //                    customPropertyQuery.Add($"{{'CustomProperties.{customProperty.Key}':{{$in:[{string.Join(',', values)}]}}}}");
            //                }
            //                match = match.Replace("#customProperties#", string.Join(",", customPropertyQuery));
            //            }
            //            else
            //            {
            //                match = match.Replace("#customProperties#", "");
            //            }

            //            List<string> stages = new List<string>();
            //            stages.Add(match);
            //            var result = await _repository.GetList<Group, Group>(stages, cancellationToken).ConfigureAwait(false);
            //            return result;
        }

        public async Task<IEnumerable<string>> GetGroupIdsByMessageKeywordAsync(IEnumerable<string> groupIds, Dictionary<string, string> searchParms, bool isExactSearch, CancellationToken cancellationToken = default)
        {
            groupIds = groupIds ?? new List<string>();
            if (searchParms != null && searchParms.Any())
            {
                var messageIds = new List<string>();
                var contentSearchParms = searchParms.Where(x => x.Key == Message.Constant.Content);
                var customPropertySearchParms = searchParms.Where(x => x.Key != Message.Constant.Content);
                if (contentSearchParms.Any())
                {
                    var sb1 = new StringBuilder();
                    if (isExactSearch)
                    {
                        var _value = contentSearchParms.ElementAt(0).Value.Replace("\\", "\\\\");
                        sb1.Append($"{Message.Constant.Content}=\"{_value}\"");
                        for (int i = 1; i < contentSearchParms.Count(); i++)
                        {
                            var index = i;
                            _value = contentSearchParms.ElementAt(i).Value.Replace("\\", "\\\\");
                            var sb2 = new StringBuilder($"{Message.Constant.Content}=\"{_value}\"");
                            sb1.Append($" || {sb2}");
                        }
                    }
                    else
                    {
                        var _value = contentSearchParms.ElementAt(0).Value.Replace("\\", "\\\\");
                        sb1.Append($"{Message.Constant.Content}.Contains(\"{_value}\")");
                        for (int i = 1; i < contentSearchParms.Count(); i++)
                        {
                            var index = i;
                            _value = contentSearchParms.ElementAt(i).Value.Replace("\\", "\\\\");
                            var sb2 = new StringBuilder($"{Message.Constant.Content}.Contains(\"{_value}\")");
                            sb1.Append($" || {sb2}");
                        }
                    }
                    var messages = await _repository.ToListAsync(_repository.Query<Domain.Message>().Where(sb1.ToString()), cancellationToken).ConfigureAwait(false);
                    messageIds = messages.Select(x => x.Id).ToList();
                }
                if (customPropertySearchParms.Any())
                {
                    var _key = customPropertySearchParms.ElementAt(0).Key.Replace("\\", "\\\\");
                    var _value = customPropertySearchParms.ElementAt(0).Value.Replace("\\", "\\\\");
                    var sb1 = new StringBuilder($"{nameof(MessageCustomProperty.Key)}=\"{_key}\" && {nameof(MessageCustomProperty.Value)} == \"{_value}\"");
                    for (int i = 1; i < customPropertySearchParms.Count(); i++)
                    {
                        var index = i;
                        _key = customPropertySearchParms.ElementAt(i).Key.Replace("\\", "\\\\");
                        _value = customPropertySearchParms.ElementAt(i).Value.Replace("\\", "\\\\");
                        var sb2 = new StringBuilder($"{nameof(MessageCustomProperty.Key)}=\"{_key}\" && {nameof(MessageCustomProperty.Value)} == \"{_value}\"");
                        sb1.Append($" || {sb2}");
                    }
                    var messageCustomProperties = await _repository.ToListAsync(_repository.Query<MessageCustomProperty>().Where(sb1.ToString()), cancellationToken).ConfigureAwait(false);
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
                if (groupIds.Any())
                {
                    return groupIds.Intersect(_groupIds).ToList();
                }
                else
                {
                    return _groupIds;
                }
            }
            else
            {
                return new List<string>();
            }

            //            var match = @"
            //{$match:
            //    {$and:[
            //        {GroupId:{$in:[#GroupIds#]}},
            //        #match_and_or#
            //    ]}
            //}
            //";
            //            var groupIdsStr = string.Join(",", groupIds.Select(x => $"'{x}'"));
            //            match = match.Replace("#GroupIds#", groupIdsStr);
            //            if (searchParms is not null && searchParms.Any())
            //            {
            //                StringBuilder match_and_or = new StringBuilder("{$or:[");

            //                foreach (var searchParm in searchParms)
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
            //                match = match.Replace("#match_and_or#", match_and_or.ToString());
            //            }
            //            else
            //            {
            //                return groupIds;
            //            }
            //            var group = "{$group:{_id:'$GroupId'}}";

            //            List<string> stages = new List<string>();
            //            stages.Add(match);
            //            stages.Add(group);

            //            var result = await _repository.GetList<Domain.Message, Group>(stages, cancellationToken).ConfigureAwait(false);
            //            return result.Select(x => x.Id);
        }
    }
}