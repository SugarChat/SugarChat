using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Shared.Paging;

namespace SugarChat.Core.Services.Friends
{
    public interface IFriendDataProvider
    {
        Task<Friend> GetByOwnIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Friend> GetByBothIdsAsync(string userId, string friendId, CancellationToken cancellationToken = default);
        Task<PagedResult<Friend>> GetAllFriendsByUserIdAsync(string userId, PageSettings pageSettings, CancellationToken cancellationToken = default);
        Task AddAsync(Friend friend, CancellationToken cancellation = default);
        Task UpdateAsync(Friend friend, CancellationToken cancellation = default);
        Task RemoveAsync(Friend friend, CancellationToken cancellation = default);
    }
}