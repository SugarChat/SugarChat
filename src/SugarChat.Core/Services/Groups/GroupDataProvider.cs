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

        public async Task<PagedResult<Group>> GetByIdsAsync(IEnumerable<string> ids, PageSettings pageSettings,
            CancellationToken cancellationToken = default)
        {
            var query = _repository.Query<Group>().Where(o => ids.Contains(o.Id))
                .OrderByDescending(o => o.LastModifyDate);
            var result = new PagedResult<Group>();
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
            if (customProperties != null)
            {
                var query = _repository.Query<GroupCustomProperty>().Where(x => groupIds.Contains(x.GroupId));
                Expression<Func<GroupCustomProperty, bool>> expression = x => x.Key == customProperties.ElementAt(0).Key && x.Value == customProperties.ElementAt(0).Value;
                //for (int i = 1; i < customProperties.Count; i++)
                //{
                //    var index = i;
                //    Expression<Func<GroupCustomProperty, bool>> _expression = x => x.Key == customProperties.ElementAt(index).Key && x.Value == customProperties.ElementAt(index).Value;
                //    expression = expression.Or(_expression);
                //}
                query = query.Where(expression);
                //foreach (var customProperty in customProperties)
                //{

                //    //query = query.Where(x => x.Key == customProperty.Key && x.Value == customProperty.Value);
                //}
                var groupCustomProperties = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
                if (groupCustomProperties.Any())
                {
                    var _groupIds = groupCustomProperties.Select(x => x.GroupId);
                    groupIds = groupIds.Intersect(_groupIds).ToList();
                }
                else
                {
                    return new List<Group>();
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
            var match = @"
{$match:
    {$and:[
        {GroupId:{$in:[#GroupIds#]}},
        #match_and_or#
    ]}
}
";
            var groupIdsStr = string.Join(",", groupIds.Select(x => $"'{x}'"));
            match = match.Replace("#GroupIds#", groupIdsStr);
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
            else
            {
                return groupIds;
            }
            var group = "{$group:{_id:'$GroupId'}}";

            List<string> stages = new List<string>();
            stages.Add(match);
            stages.Add(group);

            var result = await _repository.GetList<Domain.Message, Group>(stages, cancellationToken).ConfigureAwait(false);
            return result.Select(x => x.Id);
        }
    }
}