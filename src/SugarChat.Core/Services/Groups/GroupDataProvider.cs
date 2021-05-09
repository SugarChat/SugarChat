using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Groups
{
    public class GroupDataProvider : IGroupDataProvider
    {
        private readonly IRepository _repository;

        public GroupDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.SingleOrDefaultAsync<Group>(x => x.Id == id)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Group>> GetByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<Group>(o=>ids.Contains(o.Id)).ConfigureAwait(false);

        }
    }
}