using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Commands.Message;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

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
        Task<GetUnreadToUserFromGroupResponse> GetUnreadToUserFromGroupAsync(GetUnreadToUserFromGroupRequest request,
            CancellationToken cancellationToken = default);
        Task<GetAllToUserFromGroupResponse> GetAllToUserFromGroupAsync(GetAllToUserFromGroupRequest request,
            CancellationToken cancellationToken = default);
        Task<GetMessagesOfGroupResponse> GetMessagesOfGroupAsync(GetMessagesOfGroupRequest request,
            CancellationToken cancellationToken = default);
        Task<GetMessagesOfGroupBeforeResponse> GetMessagesOfGroupBeforeAsync(GetMessagesOfGroupBeforeRequest request,
            CancellationToken cancellationToken = default);
        Task<SetMessageReadByUserBasedOnMessageIdEvent> SetMessageReadByUserBasedOnMessageIdAsync(
            SetMessageReadByUserBasedOnMessageIdCommand command,
            CancellationToken cancellationToken = default);
        Task<SetMessageReadByUserBasedOnGroupIdEvent> SetMessageReadByUserBasedOnGroupIdAsync(
            SetMessageReadByUserBasedOnGroupIdCommand command,
            CancellationToken cancellationToken = default);
        Task<MessageRevokedEvent> RevokeMessage(RevokeMessageCommand command,
            CancellationToken cancellationToken = default);
    }
}