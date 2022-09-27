using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUserCustomProperties
{
    public class GroupUserCustomPropertyDataProvider : IGroupUserCustomPropertyDataProvider
    {
        private readonly IRepository _repository;

        public GroupUserCustomPropertyDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddRangeAsync(IEnumerable<GroupUserCustomProperty> groupUserCustomProperty, CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(groupUserCustomProperty, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUserCustomProperty>> GetPropertiesByGroupUserIds(IEnumerable<string> groupUserIds, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUserCustomProperty>(x => groupUserIds.Contains(x.GroupUserId), cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupUserCustomProperty>> GetPropertiesByGroupUserId(string groupUserId, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupUserCustomProperty>(x => x.GroupUserId == groupUserId, cancellationToken).ConfigureAwait(false);
        }

        public async Task RemoveRangeAsync(IEnumerable<GroupUserCustomProperty> groupCustomProperties, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveRangeAsync(groupCustomProperties, cancellationToken).ConfigureAwait(false);
        }
    }
}
