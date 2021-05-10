using System;
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

        public Task<Group> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return _repository.SingleOrDefaultAsync<Group>(x => x.Id == id);
        }
    }
}