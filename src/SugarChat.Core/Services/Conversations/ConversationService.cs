using AutoMapper;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Messages;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Dtos.Conversations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Event;

namespace SugarChat.Core.Services.Conversations
{
    public class ConversationService : IConversationService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IConversationDataProvider _conversationDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IMessageDataProvider _messageDataProvider;

        public ConversationService(
            IMapper mapper,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IConversationDataProvider conversationDataProvider,
            IGroupDataProvider groupDataProvider,
            IMessageDataProvider messageDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _conversationDataProvider = conversationDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
        }

        public async Task<GetConversationListResponse> GetConversationListByUserIdAsync(
            GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var groupsOfUser = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken);
            var conversations = new List<ConversationDto>();

            foreach (var group in groupsOfUser)
            {
                //Get the groups that have had conversations
                var groupMessages =
                    await _messageDataProvider.GetByGroupIdAsync(group.GroupId, cancellationToken).ConfigureAwait(false);
                if (groupMessages.Count() > 0)
                {
                    var conversationDto = new ConversationDto();
                    conversationDto.ConversationID = group.GroupId;
                    conversationDto.UnreadCount =
                       (await _messageDataProvider.GetUnreadToUserFromGroupAsync(
                            group.UserId, group.GroupId, cancellationToken)).Count();
                    conversationDto.LastMessage = _mapper.Map<MessageDto>(
                        await _messageDataProvider.GetLatestMessageOfGroupAsync(group.GroupId,
                            cancellationToken));
                    var groupDto =
                        _mapper.Map<GroupDto>(
                            await _groupDataProvider.GetByIdAsync(group.GroupId, cancellationToken));
                    groupDto.MemberCount =
                        await _groupUserDataProvider.GetGroupMemberCountAsync(group.GroupId, cancellationToken);
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
                 (await _messageDataProvider.GetUnreadToUserFromGroupAsync(
                            request.UserId, request.ConversationId, cancellationToken)).Count();
            conversationDto.LastMessage = _mapper.Map<MessageDto>(
                await _messageDataProvider.GetLatestMessageOfGroupAsync(groupUser.GroupId, cancellationToken));
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

    public async Task<GetMessageListResponse> GetPagingMessagesByConversationIdAsync(GetMessageListRequest request,
            CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.ConversationId);

            var messages = await _conversationDataProvider
                .GetPagingMessagesByConversationIdAsync(request.ConversationId, request.NextReqMessageId, request.Count,
                    cancellationToken).ConfigureAwait(false);

            return new GetMessageListResponse
            {
                Result = new MessageListResult
                {
                    Messages = _mapper.Map<IEnumerable<MessageDto>>(messages),
                    NextReqMessageID = messages.LastOrDefault()?.Id
                }
            };
        }

        public async Task<ConversationRemovedEvent> DeleteConversationByConversationIdAsync(
            RemoveConversationCommand command, CancellationToken cancellationToken = default)
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