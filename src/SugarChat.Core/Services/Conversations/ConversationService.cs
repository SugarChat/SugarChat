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

            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken, request.Type)).Select(x => x.GroupId).ToArray();
            if (request.GroupIds.Any())
            {
                groupIds = groupIds.Intersect(request.GroupIds).ToArray();
            }

            var conversations = new List<ConversationDto>();
            if (groupIds.Length == 0)
                return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Length };

            var messageCountGroupByGroupIds = await _messageDataProvider.GetMessageUnreadCountGroupByGroupIdsAsync(groupIds, user.Id, request.PageSettings, cancellationToken);

            var groupIdResults = messageCountGroupByGroupIds.Select(x => x.GroupId);
            var groups = (await _groupDataProvider.GetByIdsAsync(groupIdResults, null, cancellationToken)).Result;
            var lastMessageForGroups = await _messageDataProvider.GetLastMessageForGroupsAsync(messageCountGroupByGroupIds.Select(x => x.GroupId), cancellationToken).ConfigureAwait(false);
            foreach (var messageCountGroupByGroupId in messageCountGroupByGroupIds)
            {
                var lastMessage = lastMessageForGroups.FirstOrDefault(x => x.GroupId == messageCountGroupByGroupId.GroupId);
                var group = groups.SingleOrDefault(x => x.Id == messageCountGroupByGroupId.GroupId);
                var groupDto = _mapper.Map<GroupDto>(group);
                var conversationDto = new ConversationDto
                {
                    ConversationID = messageCountGroupByGroupId.GroupId,
                    GroupProfile = _mapper.Map<GroupDto>(group),
                    UnreadCount = messageCountGroupByGroupId.Count
                };
                if (lastMessage is not null)
                {
                    conversationDto.LastMessage = _mapper.Map<MessageDto>(lastMessage);
                }
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
                .GetPagedMessagesByConversationIdAsync(request.ConversationId, request.NextReqMessageId, request.Index, request.Count,
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

        public async Task<PagedResult<ConversationDto>> GetConversationByKeyword(GetConversationByKeywordRequest request, CancellationToken cancellationToken = default)
        {
            var conversations = new List<ConversationDto>();
            var conversationsByGroupKeyword = new List<ConversationDto>();
            var conversationsByMessageKeyword = new List<ConversationDto>();

            if ((request.GroupSearchParms == null || !request.GroupSearchParms.Any()) && (request.MessageSearchParms == null || !request.MessageSearchParms.Any()))
            {
                conversations = await _conversationDataProvider.GetConversationsByUserAsync(request.UserId, request.PageSettings, cancellationToken, request.Type).ConfigureAwait(false);
                if (!conversations.Any())
                {
                    return new PagedResult<ConversationDto> { Result = conversations, Total = 0 };
                }
            }
            else
            {
                if (request.GroupSearchParms != null && request.GroupSearchParms.Any())
                {
                    conversationsByGroupKeyword = await _conversationDataProvider.GetConversationsByGroupKeywordAsync(request.UserId, request.GroupSearchParms, cancellationToken, request.Type).ConfigureAwait(false);
                }

                if (request.MessageSearchParms != null && request.MessageSearchParms.Any())
                {
                    conversationsByMessageKeyword = await _conversationDataProvider.GetConversationsByMessageKeywordAsync(request.UserId, request.MessageSearchParms, request.IsExactSearch, cancellationToken, request.Type).ConfigureAwait(false);
                }

                if (!conversationsByGroupKeyword.Any() && !conversationsByMessageKeyword.Any())
                {
                    return new PagedResult<ConversationDto> { Result = conversations, Total = 0 };
                }

                conversations = conversationsByGroupKeyword.Union(conversationsByMessageKeyword)
                    .GroupBy(x => x.ConversationID)
                    .Select(x => x.FirstOrDefault())
                    .OrderByDescending(x => x.UnreadCount)
                    .ThenByDescending(x => x.LastMessageSentTime)
                    .Skip((request.PageSettings.PageNum - 1) * request.PageSettings.PageSize)
                    .Take(request.PageSettings.PageSize)
                    .ToList();
            }
            var groups = (await _groupDataProvider.GetByIdsAsync(conversations.Select(x => x.ConversationID), null, cancellationToken)).Result;
            var lastMessageForGroups = await _messageDataProvider.GetLastMessageForGroupsAsync(conversations.Select(x => x.ConversationID), cancellationToken).ConfigureAwait(false);
            foreach (var conversation in conversations)
            {
                var lastMessage = lastMessageForGroups.FirstOrDefault(x => x.GroupId == conversation.ConversationID);
                var group = groups.SingleOrDefault(x => x.Id == conversation.ConversationID);
                var groupDto = _mapper.Map<GroupDto>(group);
                conversation.GroupProfile = _mapper.Map<GroupDto>(group);
                if (lastMessage is not null)
                {
                    conversation.LastMessage = _mapper.Map<MessageDto>(lastMessage);
                }
            }

            return new PagedResult<ConversationDto> { Result = conversations, Total = conversations.Count };
        }
    }
}