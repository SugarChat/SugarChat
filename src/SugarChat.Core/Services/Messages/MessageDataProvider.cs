using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Messages
{
    public class MessageDataProvider : IMessageDataProvider
    {
        private readonly IRepository _repository;

        public MessageDataProvider(IRepository repository)
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

        public Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetUnreadOfUserFromFriendAsync(string userId, string friendId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllUnreadOfUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllHistoryOfUserFromFriendAsync(string userId, string friendId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Domain.Message>> GetAllHistoryOfUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public async Task<Friend> GetByUsersIdAsync(string userId, string friendId, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Friend>(x => x.UserId == userId && x.FriendId == friendId).ConfigureAwait(false);

        }

        public async Task<IEnumerable<Friend>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<Friend>(x => x.UserId == userId).ConfigureAwait(false);
        }

        public async Task AddAsync(Friend friend, CancellationToken cancellation)
        {
            await _repository.AddAsync(friend, cancellation);
        }

        public async Task UpdateAsync(Friend friend, CancellationToken cancellation)
        {
            await _repository.UpdateAsync(friend, cancellation);
        }

        public async Task RemoveAsync(Friend friend, CancellationToken cancellation)
        {
            await _repository.RemoveAsync(friend, cancellation);
        }
    }
}