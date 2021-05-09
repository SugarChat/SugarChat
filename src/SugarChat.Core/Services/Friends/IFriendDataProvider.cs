using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Friends
{
    public interface IFriendDataProvider
    {
        Task<Friend> GetByOwnIdAsync(string id, CancellationToken cancellationToken = default);
        Task<Friend> GetByUsersIdAsync(string userId, string friendId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Friend>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}