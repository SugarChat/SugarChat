using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Messages
{
    public interface IMessageDataProvider
    {
        Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetUnreadOfUserFromFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllUnreadOfUserAsync(string userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllHistoryOfUserFromFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllHistoryOfUserAsync(string userId,
            CancellationToken cancellationToken = default);

        Task AddAsync(Friend friend, CancellationToken cancellation);
        Task UpdateAsync(Friend friend, CancellationToken cancellation);
        Task RemoveAsync(Friend friend, CancellationToken cancellation);
    }
}