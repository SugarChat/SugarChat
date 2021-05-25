using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Friends
{
    public interface IFriendDataProvider
    {
        Task<Friend> GetByOwnIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Friend> GetByBothIdsAsync(string userId, string friendId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Friend>> GetAllFriendsByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task AddAsync(Friend friend, CancellationToken cancellation);
        Task UpdateAsync(Friend friend, CancellationToken cancellation);
        Task RemoveAsync(Friend friend, CancellationToken cancellation);
    }
}