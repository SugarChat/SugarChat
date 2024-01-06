using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.Messages;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Dtos;
using System.Collections.Generic;

namespace SugarChat.Core.Services.Messages
{
    public interface IMessageService : IService
    {
        Task<GetAllUnreadToUserResponse> GetAllUnreadToUserAsync(GetAllUnreadToUserRequest request,
            CancellationToken cancellationToken = default);
        Task<GetUnreadToUserFromFriendResponse> GetUnreadToUserFromFriendAsync(GetUnreadToUserFromFriendRequest request,
            CancellationToken cancellationToken = default);
        Task<GetAllHistoryToUserFromFriendResponse> GetAllHistoryToUserFromFriendAsync(
            GetAllHistoryToUserFromFriendRequest request,
            CancellationToken cancellationToken = default);
        Task<GetAllHistoryToUserResponse> GetAllHistoryToUserAsync(GetAllHistoryToUserRequest request,
            CancellationToken cancellationToken = default);
        Task<GetUnreadMessagesFromGroupResponse> GetUnreadMessagesFromGroupAsync(GetUnreadMessagesFromGroupRequest request,
            CancellationToken cancellationToken = default);
        Task<GetAllMessagesFromGroupResponse> GetAllMessagesFromGroupAsync(GetAllMessagesFromGroupRequest request,
            CancellationToken cancellationToken = default);
        Task<GetMessagesOfGroupResponse> GetMessagesOfGroupAsync(GetMessagesOfGroupRequest request,
            CancellationToken cancellationToken = default);
        Task<GetMessagesOfGroupBeforeResponse> GetMessagesOfGroupBeforeAsync(GetMessagesOfGroupBeforeRequest request,
            CancellationToken cancellationToken = default);
        Task<MessageReadSetByUserBasedOnMessageIdEvent> SetMessageReadByUserBasedOnMessageIdAsync(
            SetMessageReadByUserBasedOnMessageIdCommand command,
            CancellationToken cancellationToken = default);
        Task<MessageReadSetByUserBasedOnGroupIdEvent> SetMessageReadByUserBasedOnGroupIdAsync(
            SetMessageReadByUserBasedOnGroupIdCommand command,
            CancellationToken cancellationToken = default);

        Task SetMessageReadByUserBasedOnGroupIdAsync2(SetMessageReadByUserBasedOnGroupIdCommand command, CancellationToken cancellationToken = default);

        Task<MessageReadSetByUserIdsBasedOnGroupIdEvent> SetMessageReadByUserIdsBasedOnGroupIdAsync(
            SetMessageReadByUserIdsBasedOnGroupIdCommand command,
            CancellationToken cancellationToken = default);

        Task SetMessageReadByUserIdsBasedOnGroupIdAsync2(SetMessageReadByUserIdsBasedOnGroupIdCommand command, CancellationToken cancellationToken = default);


        Task BatchSetMessageReadByUserIdsBasedOnGroupIdAsync(BatchSetMessageReadByUserIdsBasedOnGroupIdCommand command, CancellationToken cancellationToken = default);

        Task<MessageRevokedEvent> RevokeMessageAsync(RevokeMessageCommand command,
            CancellationToken cancellationToken = default);

        Task<MessageSavedEvent> SaveMessageAsync(SendMessageCommand command,
            CancellationToken cancellationToken = default);

        Task SaveMessageAsync2(SendMessageCommand command, CancellationToken cancellationToken = default);

        Task BatchSaveMessageAsync(BatchSendMessageCommand command, CancellationToken cancellationToken = default);
        Task<GetUnreadMessageCountResponse> GetUnreadMessageCountAsync(GetUnreadMessageCountRequest request, CancellationToken cancellationToken = default);

        Task<GetMessagesByGroupIdsResponse> GetMessagesByGroupIdsAsync(GetMessagesByGroupIdsRequest request, CancellationToken cancellationToken = default);

        Task UpdateMessageDataAsync(UpdateMessageDataCommand command, CancellationToken cancellationToken = default);

        Task SetMessageUnreadByUserIdsBasedOnGroupIdAsync(SetMessageUnreadByUserIdsBasedOnGroupIdCommand command, CancellationToken cancellationToken = default);

        Task<GetUnreadMessageCountResponse> GetUnreadMessageCountAsync2(GetUnreadMessageCountRequest request, CancellationToken cancellationToken = default);
    }
}