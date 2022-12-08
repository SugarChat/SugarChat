using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Messages
{
    public interface IMessageDataProvider : IDataProvider
    {
        Task<Domain.Message> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task AddAsync(Domain.Message message, CancellationToken cancellationToken = default);
        Task UpdateAsync(Domain.Message message, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(IEnumerable<Domain.Message> message, CancellationToken cancellationToken = default);
        Task RemoveAsync(Domain.Message message, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetUnreadToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId, int groupType, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetAllHistoryToUserWithFriendAsync(string userId, string friendId,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetAllHistoryToUserAsync(string userId,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetUnreadMessagesFromGroupAsync(string userId, string groupId, string messageId = null, int? count = 15,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetAllMessagesFromGroupAsync(string groupId, int index = 0, string messageId = null, int count = 0,
            CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetMessagesOfGroupBeforeAsync(string messageId, int count,
            CancellationToken cancellationToken = default);
        Task<PagedResult<Domain.Message>> GetMessagesOfGroupAsync(string groupId, PageSettings pageSettings, DateTimeOffset? fromDate, CancellationToken cancellationToken = default);
        Task<Domain.Message>
            GetLatestMessageOfGroupAsync(string groupId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Domain.Message>> GetByGroupIdAsync(string id, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(IEnumerable<Domain.Message> messages, CancellationToken cancellationToken = default);

        Task<List<GroupUnreadCount>> GetUnreadCountByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetUserUnreadMessagesByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetMessagesByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<UnreadCountAndLastMessageByGroupId>> GetUnreadCountAndLastMessageByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<Domain.Message> GetLastMessageBygGroupIdAsync(string groupId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetLastMessageByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetListByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);

        Task<int> GetCountAsync(Expression<Func<Domain.Message, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetListAsync(PageSettings pageSettings, Expression<Func<Domain.Message, bool>> predicate = null, CancellationToken cancellationToken = default);

        Task<int> GetUnreadCountAsync(string userId,
            IEnumerable<string> filterGroupIds,
            int groupType,
            SearchGroupByGroupCustomPropertiesDto includeGroupByGroupCustomProperties,
            SearchGroupByGroupCustomPropertiesDto excludeGroupByGroupCustomProperties,
            CancellationToken cancellationToken = default);
    }
}