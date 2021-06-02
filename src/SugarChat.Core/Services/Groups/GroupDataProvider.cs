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
        private const string UpdateGroupFailed = "Group with Id {0} Update Failed.";
        private const string AddGroupFailed = "Group with Id {0} Add Failed.";
        private const string RemoveGroupFailed = "Group with Id {0} Remove Failed.";

        private readonly IRepository _repository;

        public GroupDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.SingleOrDefaultAsync<Group>(x => x.Id == id, cancellationToken)
                .ConfigureAwait(false);
            ;
        }

        public async Task<PagedResult<Group>> GetByIdsAsync(IEnumerable<string> ids, PageSettings pageSettings,
            CancellationToken cancellationToken)
        {
            var query = _repository.Query<Group>().Where(o => ids.Contains(o.Id))
                .OrderByDescending(o => o.LastModifyDate);

            var result = await _repository.ToPagedListAsync(pageSettings, query, cancellationToken);
            return result;
        }

        public async Task AddAsync(Group group, CancellationToken cancellation)
        {
            int affectedLineNum = await _repository.AddAsync(group, cancellation);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(AddGroupFailed, group.Id));
            }
        }

        public async Task UpdateAsync(Group group, CancellationToken cancellation)
        {
            int affectedLineNum = await _repository.UpdateAsync(group, cancellation);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(UpdateGroupFailed, group.Id));
            }
        }

        public async Task RemoveAsync(Group group, CancellationToken cancellation)
        {
            int affectedLineNum = await _repository.RemoveAsync(group, cancellation);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(RemoveGroupFailed, group.Id));
            }
        }
    }
}