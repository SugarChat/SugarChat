using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Users
{
    public class UserDataProvider : IUserDataProvider
    {
        private readonly IRepository _repository;

        public UserDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            return await _repository.SingleOrDefaultAsync<User>(x => x.Id == id)
                .ConfigureAwait(false);
        }
    }
}