using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Users;

namespace SugarChat.Core.Services.Friends
{
    public class FriendDataProvider : IFriendDataProvider
    {
        private readonly IRepository _repository;

        public FriendDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetRangeByIdAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<User>(o=>ids.Contains(o.Id)).ConfigureAwait(false);
        }

        public async Task<Friend> GetByOwnIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Friend>(x => x.Id == id).ConfigureAwait(false);

        }

        public async Task<Friend> GetByUsersIdAsync(string userId, string friendId, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Friend>(x => x.UserId == userId && x.FriendId == friendId).ConfigureAwait(false);

        }
    }
}