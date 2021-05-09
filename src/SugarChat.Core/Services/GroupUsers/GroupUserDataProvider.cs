using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.GroupUsers;

namespace SugarChat.Core.Services.Groups
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