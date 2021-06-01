using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Messages
{
    public interface IMessageDataProvider : IDataProvider
    {
        Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Message message, CancellationToken cancellation = default);
        Task UpdateAsync(Domain.Message message, CancellationToken cancellation = default);
        Task RemoveAsync(Domain.Message message, CancellationToken cancellation = default);
        Task<IEnumerable<Domain.Message>> GetUnreadToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllHistoryToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllHistoryToUserAsync(string userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetUnreadToUserFromGroupAsync(string userId, string groupId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetAllToUserFromGroupAsync(string userId, string groupId,
            CancellationToken cancellationToken = default);
        
        Task<IEnumerable<Domain.Message>> GetMessagesOfGroupBeforeAsync(string messageId, int count,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetMessagesOfGroupAsync(string groupId, int count, CancellationToken cancellationToken = default);
        Task<Domain.Message> GetLatestMessageOfGroupAsync(string groupId, CancellationToken cancellationToken = default);
    }
}