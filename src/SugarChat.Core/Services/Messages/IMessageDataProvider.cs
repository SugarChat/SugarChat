using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;

namespace SugarChat.Core.Services.Messages
{
    public interface IMessageDataProvider
    {
        Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Message user, CancellationToken cancellation);
        Task UpdateAsync(Domain.Message user, CancellationToken cancellation);
        Task RemoveAsync(Domain.Message user, CancellationToken cancellation);
        Task<IEnumerable<Domain.Message>> GetUnreadToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllHistoryToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllHistoryToUserAsync(string userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetUnreadToUserFromGroupAsync(string userId, string groupId,
            CancellationToken cancellationToken);

        Task<IEnumerable<Domain.Message>> GetAllToUserFromGroupAsync(string userId, string groupId,
            CancellationToken cancellationToken);
        
        Task<IEnumerable<Domain.Message>> GetMessagesOfGroupBeforeAsync(string messageId, int count,
            CancellationToken cancellationToken);

        Task<IEnumerable<Domain.Message>> GetMessagesOfGroupAsync(string requestGroupId, int requestCount, CancellationToken cancellationToken);
    }
}