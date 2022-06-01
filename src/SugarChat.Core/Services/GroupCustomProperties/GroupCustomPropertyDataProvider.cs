using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupCustomProperties
{
    public class GroupCustomPropertyDataProvider : IGroupCustomPropertyDataProvider
    {
        private readonly IRepository _repository;

        public GroupCustomPropertyDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task AddRangeAsync(IEnumerable<GroupCustomProperty> groupCustomProperty, CancellationToken cancellationToken = default)
        {
            await _repository.AddRangeAsync(groupCustomProperty, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<GroupCustomProperty>> GetPropertyByGroupId(string groupId, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<GroupCustomProperty>(x => x.GroupId == groupId, cancellationToken).ConfigureAwait(false);
        }
    }
}
