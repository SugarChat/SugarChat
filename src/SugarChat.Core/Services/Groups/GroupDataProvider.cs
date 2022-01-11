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
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Paging;

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

        public async Task<IEnumerable<Group>> GetByCustomProperties(Dictionary<string, string> customProperties, IEnumerable<string> groupIds, CancellationToken cancellationToken = default)
        {
            var match = @"
{$match:
    {$and:[
        {_id:{$in:[#GroupIds#]}},
        #customProperties#
    ]}
}
";
            if (groupIds.Count() > 0)
            {
                var groupIdsStr = string.Join(",", groupIds.Select(x => $"'{x}'"));
                match = match.Replace("#GroupIds#", groupIdsStr);
            }
            else
            {
                match = match.Replace("{_id:{$in:[#GroupIds#]}},", "");
            }

            List<string> customPropertyQuery = new List<string>();
            foreach (var customProperty in customProperties)
            {
                customPropertyQuery.Add($"{{'CustomProperties.{customProperty.Key}':/^{customProperty.Value}$/i}}");
            }
            match = match.Replace("#customProperties#", string.Join(",", customPropertyQuery));

            List<string> stages = new List<string>();
            stages.Add(match);
            var result = await _repository.GetList<Group, Group>(stages, cancellationToken).ConfigureAwait(false);
            return result;
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
                    var keyword = searchParm.Value
                               .Replace(@"\", @"\\")
                               .Replace("^", @"\^")
                               .Replace("$", @"\$")
                               .Replace(".", @"\.")
                               .Replace("*", @"\*")
                               .Replace("?", @"\?")
                               .Replace("+", @"\+")
                               .Replace("|", @"\|");
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