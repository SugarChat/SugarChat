using AutoMapper;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Dtos.Conversations;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.Conversations
{
    public class ConversationService : IConversationService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IConversationDataProvider _conversationDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;

        public ConversationService(
            IMapper mapper,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IConversationDataProvider conversationDataProvider,
            IGroupDataProvider groupDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _conversationDataProvider = conversationDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }

        public async Task<GetConversationListResponse> GetConversationListByUserIdAsync(
            GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken);
            var conversations = new List<ConversationDto>();

            foreach (var groupUser in groupUsers)
            {
                //Get the groups that have had conversations
                var groupMessages =
                    await _conversationDataProvider.GetMessagesByGroupIdAsync(groupUser.GroupId, cancellationToken);
                if (groupMessages.Count > 0)
                {
                    var conversationDto = new ConversationDto();
                    conversationDto.ConversationID = groupUser.GroupId;
                    conversationDto.UnreadCount =
                        await _conversationDataProvider.GetUserUnreadMessagesCountByGroupIdAndLastReadTimeAsync(
                            groupUser.GroupId, groupUser.LastReadTime, cancellationToken);
                    conversationDto.LastMessage = _mapper.Map<MessageDto>(
                        await _conversationDataProvider.GetLastMessageByGroupIdAsync(groupUser.GroupId,
                            cancellationToken));
                    var groupDto =
                        _mapper.Map<GroupDto>(
                            await _groupDataProvider.GetByIdAsync(groupUser.GroupId, cancellationToken));
                    groupDto.MemberCount =
                        await _groupUserDataProvider.GetGroupMemberCountAsync(groupUser.GroupId, cancellationToken);
                    conversationDto.GroupProfile = groupDto;
                    conversations.Add(conversationDto);
                }
            }

            return new GetConversationListResponse
            {
                Result = conversations
            };
        }

        public async Task<GetConversationProfileResponse> GetConversationProfileByIdAsync(
            GetConversationProfileRequest request, CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.ConversationId);

            var conversationDto = new ConversationDto();
            conversationDto.ConversationID = groupUser.GroupId;
            conversationDto.UnreadCount =
                await _conversationDataProvider.GetUserUnreadMessagesCountByGroupIdAndLastReadTimeAsync(
                    groupUser.GroupId, groupUser.LastReadTime, cancellationToken);
            conversationDto.LastMessage = _mapper.Map<MessageDto>(
                await _conversationDataProvider.GetLastMessageByGroupIdAsync(groupUser.GroupId, cancellationToken));
            var groupDto =
                _mapper.Map<GroupDto>(await _groupDataProvider.GetByIdAsync(groupUser.GroupId, cancellationToken));
            groupDto.MemberCount =
                await _groupUserDataProvider.GetGroupMemberCountAsync(groupUser.GroupId, cancellationToken);
            conversationDto.GroupProfile = groupDto;

            return new GetConversationProfileResponse
            {
                Result = conversationDto
            };
        }

        public async Task<MessageReadEvent> SetMessageAsReadByConversationIdAsync(SetMessageAsReadCommand command,
            CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(command.UserId, command.ConversationId);

            var lastMessage =
                await _conversationDataProvider.GetLastMessageByGroupIdAsync(command.ConversationId, cancellationToken);
            groupUser.LastReadTime = lastMessage.SentTime;
            await _groupUserDataProvider.UpdateAsync(groupUser, cancellationToken);

            return _mapper.Map<MessageReadEvent>(command);
        }

        public async Task<GetMessageListResponse> GetPagingMessagesByConversationIdAsync(GetMessageListRequest request,
            CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.ConversationId);

            var (messages, nextReqMessageId) = await _conversationDataProvider
                .GetPagingMessagesByConversationIdAsync(request.ConversationId, request.NextReqMessageId, request.Count,
                    cancellationToken).ConfigureAwait(false);

            return new GetMessageListResponse
            {
                Result = new MessageListResult
                {
                    Messages = _mapper.Map<IEnumerable<MessageDto>>(messages),
                    NextReqMessageID = nextReqMessageId
                }
            };
        }

        public async Task<ConversationRemovedEvent> DeleteConversationByConversationIdAsync(
            DeleteConversationCommand command, CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(command.UserId, command.ConversationId);

            await _groupUserDataProvider.RemoveAsync(groupUser, cancellationToken);

            return _mapper.Map<ConversationRemovedEvent>(command);
        }
    }
}