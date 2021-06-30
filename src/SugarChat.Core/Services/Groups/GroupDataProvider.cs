using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Shared.Paging;

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

        public async Task<IEnumerable<Group>> GetByCustomPropertys(Dictionary<string, string> customPropertys, IEnumerable<string> groupIds)
        {
            if (groupIds is null || groupIds.Count() == 0)
            {
                throw new BusinessWarningException(Prompt.ParameterRequired.WithParams("groupIds"));
            }
            var groups = await _repository.ToListAsync<Group>(x => groupIds.Contains(x.Id));
            List<Group> filterGroups = new List<Group>();
            if (customPropertys is not null)
            {
                foreach (var group in groups)
                {
                    if (group.CustomProperties is not null)
                    {
                        bool isAdd = true;
                        foreach (var customProperty in customPropertys)
                        {
                            if (group.CustomProperties.GetValueOrDefault(customProperty.Key) != customProperty.Value)
                            {
                                isAdd = false;
                                break;
                            }
                        }
                        if (isAdd)
                        {
                            filterGroups.Add(group);
                        }
                    }
                }
            }
            return filterGroups;
        }
    }
}