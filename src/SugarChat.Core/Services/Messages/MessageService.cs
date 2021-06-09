using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Commands.Message;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;

namespace SugarChat.Core.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IMessageDataProvider _messageDataProvider;
        private readonly IFriendDataProvider _friendDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public MessageService(IMapper mapper, IUserDataProvider userDataProvider,
            IMessageDataProvider messageDataProvider,
            IFriendDataProvider friendDataProvider, IGroupDataProvider groupDataProvider,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _messageDataProvider = messageDataProvider;
            _friendDataProvider = friendDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }


        private Task<User> GetUserAsync(string id, CancellationToken cancellationToken = default)
        {
            return _userDataProvider.GetByIdAsync(id, cancellationToken);
        }

        public async Task<GetAllUnreadToUserResponse> GetAllUnreadToUserAsync(GetAllUnreadToUserRequest request,
            CancellationToken cancellationToken = default)
        {
            string userId = request.UserId;
            User user = await GetUserAsync(userId, cancellationToken);
            user.CheckExist(userId);

            return new GetAllUnreadToUserResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(
                    await _messageDataProvider.GetAllUnreadToUserAsync(userId, cancellationToken))
            };
        }

        public async Task<GetUnreadToUserFromFriendResponse> GetUnreadToUserFromFriendAsync(
            GetUnreadToUserFromFriendRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            user = await GetUserAsync(request.FriendId, cancellationToken);
            user.CheckExist(request.FriendId);

            Friend friend =
                await _friendDataProvider.GetByBothIdsAsync(request.UserId, request.FriendId, cancellationToken);
            friend.CheckExist(request.UserId, request.FriendId);

            return new GetUnreadToUserFromFriendResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(
                    await _messageDataProvider.GetUnreadToUserWithFriendAsync(request.UserId, request.FriendId,
                        cancellationToken))
            };
        }

        public async Task<GetAllHistoryToUserFromFriendResponse> GetAllHistoryToUserFromFriendAsync(
            GetAllHistoryToUserFromFriendRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            user = await GetUserAsync(request.FriendId, cancellationToken);
            user.CheckExist(request.FriendId);

            Friend friend =
                await _friendDataProvider.GetByBothIdsAsync(request.UserId, request.FriendId, cancellationToken);
            friend.CheckExist(request.UserId, request.FriendId);
            
            return new GetAllHistoryToUserFromFriendResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(
                    await _messageDataProvider.GetAllHistoryToUserWithFriendAsync(request.UserId, request.FriendId,
                        cancellationToken))
            };
        }

        public async Task<GetAllHistoryToUserResponse> GetAllHistoryToUserAsync(GetAllHistoryToUserRequest request,
            CancellationToken cancellationToken = default)
        {
            string userId = request.UserId;
            User user = await GetUserAsync(userId, cancellationToken);
            user.CheckExist(userId);

            return new GetAllHistoryToUserResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(
                    await _messageDataProvider.GetAllHistoryToUserAsync(userId, cancellationToken))
            };
        }

        public async Task<GetUnreadToUserFromGroupResponse> GetUnreadToUserFromGroupAsync(
            GetUnreadToUserFromGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            Group group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken);
            group.CheckExist(request.GroupId);

            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.GroupId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.GroupId);

            return new GetUnreadToUserFromGroupResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(
                    await _messageDataProvider.GetUnreadToUserFromGroupAsync(request.UserId, request.GroupId,
                        cancellationToken))
            };
        }

        public async Task<GetAllToUserFromGroupResponse> GetAllToUserFromGroupAsync(
            GetAllToUserFromGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            Group group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken);
            group.CheckExist(request.GroupId);

            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.GroupId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.GroupId);

            return new GetAllToUserFromGroupResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(
                    await _messageDataProvider.GetAllToUserFromGroupAsync(request.UserId, request.GroupId,
                        cancellationToken))
            };
        }

        public async Task<GetMessagesOfGroupResponse> GetMessagesOfGroupAsync(GetMessagesOfGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken);
            group.CheckExist(request.GroupId);

            var messages =
                await _messageDataProvider.GetMessagesOfGroupAsync(request.GroupId, request.Count, cancellationToken);
            return new()
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
            };
        }

        public async Task<GetMessagesOfGroupBeforeResponse> GetMessagesOfGroupBeforeAsync(
            GetMessagesOfGroupBeforeRequest request,
            CancellationToken cancellationToken = default)
        {
            Domain.Message message = await _messageDataProvider.GetByIdAsync(request.MessageId, cancellationToken);
            message.CheckExist(request.MessageId);

            var messages =
                await _messageDataProvider.GetMessagesOfGroupBeforeAsync(request.MessageId, request.Count,
                    cancellationToken);
            return new()
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
            };
        }

        public async Task<MessageReadSetByUserBasedOnMessageIdEvent> SetMessageReadByUserBasedOnMessageIdAsync(
            SetMessageReadByUserBasedOnMessageIdCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(command.UserId, cancellationToken);
            user.CheckExist(command.UserId);
            Domain.Message message = await _messageDataProvider.GetByIdAsync(command.MessageId, cancellationToken);
            message.CheckExist(command.MessageId);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, message.GroupId,
                    cancellationToken);
            groupUser.CheckExist(command.UserId, message.GroupId);
            groupUser.CheckLastReadTimeEarlierThan(message.SentTime);

            await _groupUserDataProvider.SetMessageReadAsync(command.UserId, message.GroupId, message.SentTime,
                cancellationToken);
            return _mapper.Map<MessageReadSetByUserBasedOnMessageIdEvent>(command);
        }

        public async Task<MessageReadSetByUserBasedOnGroupIdEvent> SetMessageReadByUserBasedOnGroupIdAsync(
            SetMessageReadByUserBasedOnGroupIdCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(command.UserId, cancellationToken);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken);
            group.CheckExist(command.GroupId);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId,
                    cancellationToken);
            groupUser.CheckExist(command.UserId, command.GroupId);
            Domain.Message lastMessageOfGroup =
                await _messageDataProvider.GetLatestMessageOfGroupAsync(command.GroupId, cancellationToken);
            groupUser.CheckLastReadTimeEarlierThan(lastMessageOfGroup.SentTime);

            await _groupUserDataProvider.SetMessageReadAsync(command.UserId, command.GroupId,
                lastMessageOfGroup.SentTime,
                cancellationToken);
            return _mapper.Map<MessageReadSetByUserBasedOnGroupIdEvent>(command);

        }

        public async Task<MessageRevokedEvent> RevokeMessageAsync(RevokeMessageCommand command,
            CancellationToken cancellationToken = default)
        {
            var message = await _messageDataProvider.GetByIdAsync(command.MessageId).ConfigureAwait(false);
            message.CheckExist(command.MessageId);
            if (message.SentBy != command.UserId)
            {
                throw new BusinessWarningException(Prompt.RevokeOthersMessage.WithParams(command.UserId, command.MessageId));
            }

            if (message.SentTime.Add(command.RevokeTimeLimit) < DateTime.Now)
            {
                throw new BusinessWarningException(Prompt.TooLateToRevoke.WithParams(command.UserId, command.MessageId));
            }

            message.IsRevoked = true;
            await _messageDataProvider.UpdateAsync(message, cancellationToken);

            return _mapper.Map<MessageRevokedEvent>(command);
        }
    }
}