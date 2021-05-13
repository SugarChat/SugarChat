using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserDataProvider : IGroupUserDataProvider
    {
        private readonly IRepository _repository;

        public GroupUserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<GroupUser>> GetByUserIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.ToListAsync<GroupUser>(o=>o.UserId == id).ConfigureAwait(false);

        }
    }
}