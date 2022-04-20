﻿using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using System;
using System.Collections.Generic;
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
        Task<IEnumerable<Domain.Message>> GetAllUnreadToUserAsync(string userId,
            CancellationToken cancellationToken = default);
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

        Task<int> GetUnreadMessageCountAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetUserUnreadMessagesByGroupIdsAsync(string userId, IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetMessagesByGroupIdsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetByGroupIdsAsync(string[] groupIds, CancellationToken cancellationToken);

        Task<IEnumerable<MessageCountGroupByGroupId>> GetMessageUnreadCountGroupByGroupIdsAsync(IEnumerable<string> groupIds, string userId, PageSettings pageSettings, CancellationToken cancellationToken = default);

        Task<Domain.Message> GetLastMessageBygGroupIdAsync(string groupId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetLastMessageForGroupsAsync(IEnumerable<string> groupIds, CancellationToken cancellationToken = default);

        Task<IEnumerable<Domain.Message>> GetListByIdsAsync(IEnumerable<string> ids, CancellationToken cancellationToken = default);
    }
}