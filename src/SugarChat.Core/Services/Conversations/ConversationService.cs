using AutoMapper;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Messages;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Conversations;
using SugarChat.Message.Events.Conversations;
using SugarChat.Message.Requests.Conversations;
using SugarChat.Message.Responses.Conversations;
using SugarChat.Message.Dtos;
using SugarChat.Message.Dtos.Conversations;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Message.Paging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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

        public async Task<PagedResult<ConversationDto>> GetConversationListByUserIdAsync(GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken)).Select(x => x.GroupId).ToArray();
            if (request.GroupIds.Any())
            {
                groupIds = groupIds.Intersect(request.GroupIds).ToArray();
            }

            var conversations = new List<ConversationDto>();
            if (groupIds.Length == 0)
                return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Length };

            var messageCountGroupByGroupIds = _messageDataProvider.GetMessageCountGroupByGroupId(groupIds, user.Id, request.PageSettings);

            var groupIdResults = messageCountGroupByGroupIds.Select(x => x.GroupId);
            var groups = (await _groupDataProvider.GetByIdsAsync(groupIdResults, request.PageSettings, cancellationToken)).Result;
            foreach (var messageCountGroupByGroupId in messageCountGroupByGroupIds)
            {
                var lastMessage = _messageDataProvider.GetLastMessageBygGroupId(messageCountGroupByGroupId.GroupId);
                var group = groups.SingleOrDefault(x => x.Id == messageCountGroupByGroupId.GroupId);
                var groupDto = _mapper.Map<GroupDto>(group);
                var conversationDto = new ConversationDto
                {
                    ConversationID = messageCountGroupByGroupId.GroupId,
                    GroupProfile = _mapper.Map<GroupDto>(group),
                    LastMessage = _mapper.Map<MessageDto>(lastMessage),
                    UnreadCount = messageCountGroupByGroupId.Count
                };
                conversations.Add(conversationDto);
            }

            return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Length };
        }

        public async Task<GetConversationProfileResponse> GetConversationProfileByIdAsync(
            GetConversationProfileRequest request, CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.ConversationId);

            var conversationDto = await GetConversationDto(groupUser, cancellationToken);

            return new GetConversationProfileResponse
            {
                Result = conversationDto
            };
        }

        public async Task<GetMessageListResponse> GetPagedMessagesByConversationIdAsync(GetMessageListRequest request,
            CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.ConversationId);

            var messages = await _conversationDataProvider
                .GetPagedMessagesByConversationIdAsync(request.ConversationId, request.NextReqMessageId, request.PagaIndex, request.Count,
                    cancellationToken).ConfigureAwait(false);

            return new GetMessageListResponse
            {
                Messages = messages.Select(x => _mapper.Map<MessageDto>(x)).ToList(),
                NextReqMessageID = messages.LastOrDefault()?.Id
            };
        }

        public async Task<ConversationRemovedEvent> RemoveConversationByConversationIdAsync(
            RemoveConversationCommand command, CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(command.UserId, command.ConversationId);

            await _groupUserDataProvider.RemoveAsync(groupUser, cancellationToken);

            return _mapper.Map<ConversationRemovedEvent>(command);
        }


        private async Task<ConversationDto> GetConversationDto(GroupUser groupUser,
            CancellationToken cancellationToken = default)
        {
            var conversationDto = new ConversationDto();
            conversationDto.ConversationID = groupUser.GroupId;
            conversationDto.UnreadCount =
                (await _messageDataProvider.GetUnreadMessagesFromGroupAsync(
                    groupUser.UserId, groupUser.GroupId, cancellationToken: cancellationToken)).Count();
            conversationDto.LastMessage = _mapper.Map<MessageDto>(
                await _messageDataProvider.GetLatestMessageOfGroupAsync(groupUser.GroupId, cancellationToken));
            var groupDto =
                _mapper.Map<GroupDto>(await _groupDataProvider.GetByIdAsync(groupUser.GroupId, cancellationToken));
            groupDto.MemberCount =
                await _groupUserDataProvider.GetGroupMemberCountByGroupIdAsync(groupUser.GroupId, cancellationToken);
            conversationDto.GroupProfile = groupDto;
            return conversationDto;
        }

        public async Task<PagedResult<ConversationDto>> GetConversationByKeyword(GetConversationByKeywordRequest request, CancellationToken cancellationToken)
        {
            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken)).Select(x => x.GroupId).ToArray();
            if (request.GroupIds.Any())
            {
                groupIds = groupIds.Intersect(request.GroupIds).ToArray();
            }

            var conversations = new List<ConversationDto>();
            if (groupIds.Length == 0)
                return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Length };

            var filterGroupIds = _groupDataProvider.GetGroupIdsByMessageKeyword(groupIds, request.SearchParms, request.PageSettings, request.IsExactSearch);
            var messageCountGroupByGroupIds = _messageDataProvider.GetMessageCountGroupByGroupId(filterGroupIds, request.UserId, request.PageSettings);

            var groupIdResults = messageCountGroupByGroupIds.Select(x => x.GroupId);
            var groups = (await _groupDataProvider.GetByIdsAsync(groupIdResults, request.PageSettings, cancellationToken)).Result;
            foreach (var messageCountGroupByGroupId in messageCountGroupByGroupIds)
            {
                var lastMessage = _messageDataProvider.GetLastMessageBygGroupId(messageCountGroupByGroupId.GroupId);
                var group = groups.SingleOrDefault(x => x.Id == messageCountGroupByGroupId.GroupId);
                var groupDto = _mapper.Map<GroupDto>(group);
                var conversationDto = new ConversationDto
                {
                    ConversationID = messageCountGroupByGroupId.GroupId,
                    GroupProfile = _mapper.Map<GroupDto>(group),
                    LastMessage = _mapper.Map<MessageDto>(lastMessage),
                    UnreadCount = messageCountGroupByGroupId.Count
                };
                conversations.Add(conversationDto);
            }

            return new PagedResult<ConversationDto> { Result = conversations, Total = filterGroupIds.Count() };
        }
    }
}