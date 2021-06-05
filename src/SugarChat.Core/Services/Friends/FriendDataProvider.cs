using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.IRepositories;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Services.Friends
{
    public class FriendDataProvider : IFriendDataProvider
    {
        private readonly IRepository _repository;

        public FriendDataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetRangeByIdAsync(IEnumerable<string> ids,
            CancellationToken cancellationToken = default)
        {
            return await _repository.ToListAsync<User>(o => ids.Contains(o.Id), cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Friend> GetByOwnIdAsync(string id, CancellationToken cancellationToken = default)
        {
            return await _repository.SingleOrDefaultAsync<Friend>(x => x.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<Friend> GetByBothIdsAsync(string userId, string friendId,
            CancellationToken cancellationToken = default)
        {
            return await _repository
                .SingleOrDefaultAsync<Friend>(x => x.UserId == userId && x.FriendId == friendId, cancellationToken)
                .ConfigureAwait(false);
        }

        public Task<PagedResult<Friend>> GetAllFriendsByUserIdAsync(string userId,
            PageSettings pageSettings,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Friend> query = _repository.Query<Friend>().Where(o => o.UserId == userId)
                .OrderByDescending(o => o.BecomeFriendAt);
            int total = query.Count();
            IEnumerable<Friend> friends = query.Paging(pageSettings).ToList();
            return Task.FromResult(new PagedResult<Friend>
            {
                Result = friends,
                Total = total
            });
        }

        public async Task AddAsync(Friend friend, CancellationToken cancellation)
        {
            int affectedLineNum = await _repository.AddAsync(friend, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.AddFriendFailed, friend.Id));
            }
        }

        public async Task UpdateAsync(Friend friend, CancellationToken cancellation)
        {
            int affectedLineNum = await _repository.UpdateAsync(friend, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.UpdateFriendFailed, friend.Id));
            }
        }

        public async Task RemoveAsync(Friend friend, CancellationToken cancellation)
        {
            int affectedLineNum = await _repository.RemoveAsync(friend, cancellation).ConfigureAwait(false);
            if (affectedLineNum != 1)
            {
                throw new BusinessWarningException(string.Format(ExceptionPrompt.RemoveFriendFailed, friend.Id));
            }
        }

        public async Task<bool> AreFriendsAsync(string userId, string friendId,
            CancellationToken cancellationToken = default)
        {
            return await _repository.AnyAsync<Friend>(o => o.UserId == userId && o.FriendId == friendId,
                cancellationToken).ConfigureAwait(false);
        }
    }
}