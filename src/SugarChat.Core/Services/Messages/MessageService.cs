using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Message.Exceptions;
using SugarChat.Core.Services.Friends;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Events.Messages;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Message.Dtos;
using SugarChat.Message.Requests.Messages;
using SugarChat.Message.Responses.Messages;
using System.Linq;
using SugarChat.Core.Services.Configurations;
using SugarChat.Message.Paging;
using SugarChat.Core.Services.MessageCustomProperties;
using SugarChat.Core.IRepositories;
using Serilog;

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
        private readonly IConfigurationDataProvider _configurationDataProvider;
        private readonly IMessageCustomPropertyDataProvider _messageCustomPropertyDataProvider;
        private readonly ITransactionManager _transactionManagement;

        public MessageService(IMapper mapper, IUserDataProvider userDataProvider,
            IMessageDataProvider messageDataProvider,
            IFriendDataProvider friendDataProvider, IGroupDataProvider groupDataProvider,
            IGroupUserDataProvider groupUserDataProvider, IConfigurationDataProvider configurationDataProvider,
            IMessageCustomPropertyDataProvider messageCustomPropertyDataProvider,
            ITransactionManager transactionManagement)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _messageDataProvider = messageDataProvider;
            _friendDataProvider = friendDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _configurationDataProvider = configurationDataProvider;
            _messageCustomPropertyDataProvider = messageCustomPropertyDataProvider;
            _transactionManagement = transactionManagement;
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

            var messages = await _messageDataProvider.GetAllUnreadToUserAsync(userId, cancellationToken, request.Type);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return new GetAllUnreadToUserResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
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

            var messages = await _messageDataProvider.GetUnreadToUserWithFriendAsync(request.UserId, request.FriendId, cancellationToken);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return new GetUnreadToUserFromFriendResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
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

            var messages = await _messageDataProvider.GetAllHistoryToUserWithFriendAsync(request.UserId, request.FriendId, cancellationToken);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return new GetAllHistoryToUserFromFriendResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
            };
        }

        public async Task<GetAllHistoryToUserResponse> GetAllHistoryToUserAsync(GetAllHistoryToUserRequest request,
            CancellationToken cancellationToken = default)
        {
            string userId = request.UserId;
            User user = await GetUserAsync(userId, cancellationToken);
            user.CheckExist(userId);

            var messages = await _messageDataProvider.GetAllHistoryToUserAsync(userId, cancellationToken);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return new GetAllHistoryToUserResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
            };
        }

        public async Task<GetUnreadMessagesFromGroupResponse> GetUnreadMessagesFromGroupAsync(
            GetUnreadMessagesFromGroupRequest request,
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

            var messages = await _messageDataProvider.GetUnreadMessagesFromGroupAsync(request.UserId, request.GroupId, request.MessageId, request.Count, cancellationToken);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return new GetUnreadMessagesFromGroupResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
            };
        }

        public async Task<GetAllMessagesFromGroupResponse> GetAllMessagesFromGroupAsync(
            GetAllMessagesFromGroupRequest request,
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

            var messages = await _messageDataProvider.GetAllMessagesFromGroupAsync(request.GroupId, request.Index, request.MessageId, request.Count, cancellationToken);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return new GetAllMessagesFromGroupResponse
            {
                Messages = _mapper.Map<IEnumerable<MessageDto>>(messages)
            };
        }

        public async Task<GetMessagesOfGroupResponse> GetMessagesOfGroupAsync(GetMessagesOfGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken);
            group.CheckExist(request.GroupId);

            var messages = await _messageDataProvider.GetMessagesOfGroupAsync(request.GroupId, request.PageSettings, request.FromDate, cancellationToken);
            await GetPropertiesForMessages(messages.Result, cancellationToken).ConfigureAwait(false);
            return new()
            {
                Messages = new PagedResult<MessageDto>
                {
                    Result = _mapper.Map<IEnumerable<MessageDto>>(messages.Result),
                    Total = messages.Total
                }
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
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
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

            GroupUser groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken);
            groupUser.CheckExist(command.UserId, command.GroupId);

            Domain.Message lastMessageOfGroup = await _messageDataProvider.GetLatestMessageOfGroupAsync(command.GroupId, cancellationToken);
            if (lastMessageOfGroup is null)
            {
                return _mapper.Map<MessageReadSetByUserBasedOnGroupIdEvent>(command);
            }
            else
            {
                DateTimeOffset lastMessageSentTime = lastMessageOfGroup.SentTime;
                groupUser.CheckLastReadTimeEarlierThan(lastMessageSentTime);
                await _groupUserDataProvider.SetMessageReadAsync(command.UserId, command.GroupId, lastMessageSentTime, cancellationToken);
                return _mapper.Map<MessageReadSetByUserBasedOnGroupIdEvent>(command);
            }
        }

        public async Task<MessageReadSetByUserIdsBasedOnGroupIdEvent> SetMessageReadByUserIdsBasedOnGroupIdAsync(
            SetMessageReadByUserIdsBasedOnGroupIdCommand command,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken).ConfigureAwait(false);
            group.CheckExist(command.GroupId);

            IEnumerable<GroupUser> groupUsers = (await _groupUserDataProvider.GetMembersByGroupIdAsync(command.GroupId, cancellationToken).ConfigureAwait(false)).ToList();
            var groupUserIds = groupUsers.Select(o => o.UserId);
            if (command.UserIds.Except(groupUserIds).Any())
            {
                throw new BusinessWarningException(Prompt.NotAllGroupUsersExist);
            }

            Domain.Message lastMessageOfGroup = await _messageDataProvider.GetLatestMessageOfGroupAsync(command.GroupId, cancellationToken).ConfigureAwait(false);
            if (lastMessageOfGroup is not null)
            {
                DateTimeOffset lastMessageSentTime = lastMessageOfGroup.SentTime;
                foreach (GroupUser groupUser in groupUsers)
                {
                    groupUser.CheckLastReadTimeEarlierThan(lastMessageSentTime);
                }

                await _groupUserDataProvider.SetMessageReadByIdsAsync(command.UserIds, command.GroupId, lastMessageSentTime,
                    cancellationToken).ConfigureAwait(false);
            }

            return _mapper.Map<MessageReadSetByUserIdsBasedOnGroupIdEvent>(command);
        }

        public async Task<MessageRevokedEvent> RevokeMessageAsync(RevokeMessageCommand command,
            CancellationToken cancellationToken = default)
        {
            DateTimeOffset now = DateTimeOffset.Now;
            var message = await _messageDataProvider.GetByIdAsync(command.MessageId, cancellationToken).ConfigureAwait(false);
            message.CheckExist(command.MessageId);
            if (message.SentBy != command.UserId)
            {
                throw new BusinessWarningException(Prompt.RevokeOthersMessage.WithParams(command.UserId, command.MessageId));
            }

            var revokeTimeLimitInMinutes =
                (await _configurationDataProvider.GetServerConfigurationsAsync(cancellationToken)).RevokeTimeLimitInMinutes;
            if (message.SentTime.AddMinutes(revokeTimeLimitInMinutes) < now)
            {
                throw new BusinessWarningException(Prompt.TooLateToRevoke.WithParams(command.UserId, command.MessageId));
            }

            message.IsRevoked = true;
            await _messageDataProvider.UpdateAsync(message, cancellationToken);

            return _mapper.Map<MessageRevokedEvent>(command);
        }

        public async Task<MessageSavedEvent> SaveMessageAsync(SendMessageCommand command, CancellationToken cancellationToken = default)
        {
            Domain.Message message = _mapper.Map<Domain.Message>(command);
            message.SentTime = DateTime.Now;

            var messageCustomProperties = new List<MessageCustomProperty>();
            if (command.CustomProperties != null && command.CustomProperties.Any())
            {
                foreach (var customProperty in command.CustomProperties)
                {
                    messageCustomProperties.Add(new MessageCustomProperty
                    {
                        MessageId = command.Id,
                        Key = customProperty.Key,
                        Value = customProperty.Value
                    });
                }
            }
            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _messageCustomPropertyDataProvider.AddRangeAsync(messageCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _messageDataProvider.AddAsync(message, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
            return _mapper.Map<MessageSavedEvent>(command);
        }

        public async Task<GetUnreadMessageCountResponse> GetUnreadMessageCountAsync(GetUnreadMessageCountRequest request, CancellationToken cancellationToken = default)
        {
            string userId = request.UserId;
            User user = await GetUserAsync(userId, cancellationToken);
            user.CheckExist(userId);

            return new GetUnreadMessageCountResponse
            {
                Count = await _messageDataProvider.GetUnreadMessageCountAsync(request.UserId, request.GroupIds, cancellationToken).ConfigureAwait(false)
            };
        }

        public async Task<IEnumerable<MessageDto>> GetMessagesByGroupIdsAsync(GetMessagesByGroupIdsRequest request, CancellationToken cancellationToken = default)
        {
            User user = await GetUserAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var messages = await _messageDataProvider.GetMessagesByGroupIdsAsync(request.GroupIds, cancellationToken);
            await GetPropertiesForMessages(messages, cancellationToken).ConfigureAwait(false);
            return messages.Select(x => _mapper.Map<MessageDto>(x)).ToArray();
        }

        public async Task UpdateMessageDataAsync(UpdateMessageDataCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);

            var ids = command.Messages.Select(x => x.Id).ToArray();
            var messages = await _messageDataProvider.GetListByIdsAsync(ids, cancellationToken).ConfigureAwait(false);
            var groups = (await _groupDataProvider.GetByIdsAsync(messages.Select(x => x.GroupId), null, cancellationToken).ConfigureAwait(false)).Result;
            foreach (var message in messages)
            {
                var group = groups.SingleOrDefault(x => x.Id == message.GroupId);
                group.CheckExist(message.GroupId);
            }
            var userIds = messages.Select(x => x.SentBy);
            var users = await _userDataProvider.GetListAsync(x => userIds.Contains(x.Id));
            foreach (var message in messages)
            {
                var _user = users.SingleOrDefault(x => x.Id == message.SentBy);
                _user.CheckExist(message.SentBy);
            }
            var messageCustomProperties = new List<MessageCustomProperty>();
            foreach (var updateMessageDto in command.Messages)
            {
                var message = messages.FirstOrDefault(x => x.Id == updateMessageDto.Id);
                if (message != null)
                {
                    _mapper.Map(updateMessageDto, message);
                }
                if (updateMessageDto.CustomProperties != null && updateMessageDto.CustomProperties.Any())
                {
                    foreach (var customProperty in updateMessageDto.CustomProperties)
                    {
                        messageCustomProperties.Add(new MessageCustomProperty
                        {
                            MessageId = updateMessageDto.Id,
                            Key = customProperty.Key,
                            Value = customProperty.Value
                        });
                    }
                }
            }
            var oldMessageCustomProperties = await _messageCustomPropertyDataProvider.GetPropertiesByMessageIds(command.Messages.Select(x => x.Id), cancellationToken).ConfigureAwait(false);
            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _messageCustomPropertyDataProvider.RemoveRangeAsync(oldMessageCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _messageCustomPropertyDataProvider.AddRangeAsync(messageCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _messageDataProvider.UpdateRangeAsync(messages, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }
        }

        private async Task GetPropertiesForMessages(IEnumerable<Domain.Message> messages, CancellationToken cancellationToken)
        {
            var messageIds = messages.Select(x => x.Id);
            var messageCustomProperties = await _messageCustomPropertyDataProvider.GetPropertiesByMessageIds(messageIds, cancellationToken).ConfigureAwait(false);
            foreach (var message in messages)
            {
                var _messageCustomProperties = messageCustomProperties.Where(x => x.Id == message.Id).ToList();
                message.CustomPropertyList = _messageCustomProperties;
            }
        }

        public async Task MigrateCustomProperty(CancellationToken cancellation = default)
        {
            var total = await _messageDataProvider.GetCountAsync(x => x.CustomProperties != null && x.CustomProperties != new Dictionary<string, string>(), cancellation).ConfigureAwait(false);
            var pageSize = 5000;
            var pageIndex = total / pageSize + 1;
            for (int i = 1; i <= pageIndex; i++)
            {
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
                {
                    try
                    {
                        var messages = await _messageDataProvider.GetListAsync(new PageSettings { PageNum = 1, PageSize = pageSize }, x => x.CustomProperties != null && x.CustomProperties != new Dictionary<string, string>(), cancellation).ConfigureAwait(false);
                        var messageCustomProperties = new List<MessageCustomProperty>();
                        foreach (var message in messages)
                        {
                            foreach (var customProperty in message.CustomProperties)
                            {
                                messageCustomProperties.Add(new MessageCustomProperty { MessageId = message.Id, Key = customProperty.Key, Value = customProperty.Value });
                            }
                            //message.CustomProperties = null;
                        }
                        //await _messageDataProvider.UpdateRangeAsync(messages, cancellation).ConfigureAwait(false);
                        await _messageCustomPropertyDataProvider.AddRangeAsync(messageCustomProperties, cancellation).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Migrate Message CustomProperty Error");
                        await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}