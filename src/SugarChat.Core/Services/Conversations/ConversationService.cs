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
using SugarChat.Core.Services.GroupCustomProperties;
using SugarChat.Core.Services.MessageCustomProperties;

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
        private readonly IGroupCustomPropertyDataProvider _groupCustomPropertyDataProvider;
        private readonly IMessageCustomPropertyDataProvider _messageCustomPropertyDataProvider;

        public ConversationService(
            IMapper mapper,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IConversationDataProvider conversationDataProvider,
            IGroupDataProvider groupDataProvider,
            IMessageDataProvider messageDataProvider,
            IGroupCustomPropertyDataProvider groupCustomPropertyDataProvider,
            IMessageCustomPropertyDataProvider messageCustomPropertyDataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _conversationDataProvider = conversationDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
            _groupCustomPropertyDataProvider = groupCustomPropertyDataProvider;
            _messageCustomPropertyDataProvider = messageCustomPropertyDataProvider;
        }

        public async Task<PagedResult<ConversationDto>> GetConversationListByUserIdAsync(GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, request.GroupIds, request.GroupType, cancellationToken)).Select(x => x.GroupId).ToList();

            var includeGroupIdsByCustomProperties = await _groupDataProvider.GetGroupIdByIncludeCustomPropertiesAsync(groupIds, request.IncludeGroupByGroupCustomProperties, cancellationToken).ConfigureAwait(false);
            groupIds = groupIds.Where(x => includeGroupIdsByCustomProperties.Contains(x)).ToList();

            var excludeGroupIdsByCustomProperties = await _groupDataProvider.GetGroupIdByExcludeCustomPropertiesAsync(groupIds, request.ExcludeGroupByGroupCustomProperties, cancellationToken).ConfigureAwait(false);
            groupIds = groupIds.Where(x => !excludeGroupIdsByCustomProperties.Contains(x)).ToList();

            if (!groupIds.Any())
                return new PagedResult<ConversationDto> { Result = new List<ConversationDto>(), Total = 0 };

            var unreadCountAndLastMessageByGroupIds = await _messageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync(request.UserId,
                    groupIds,
                    request.PageSettings,
                    request.GroupType,
                    cancellationToken).ConfigureAwait(false);

            var conversations = new List<ConversationDto>();
            if (!unreadCountAndLastMessageByGroupIds.Any())
                return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Count };

            var groupIdResults = unreadCountAndLastMessageByGroupIds.Select(x => x.GroupId);
            var groups = (await _groupDataProvider.GetByIdsAsync(groupIdResults, null, cancellationToken)).Result;
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groupIdResults, cancellationToken).ConfigureAwait(false);

            foreach (var unreadCountAndLastMessageByGroupId in unreadCountAndLastMessageByGroupIds)
            {
                var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == unreadCountAndLastMessageByGroupId.GroupId).ToList();
                var group = groups.SingleOrDefault(x => x.Id == unreadCountAndLastMessageByGroupId.GroupId);
                var groupDto = _mapper.Map<GroupDto>(group);
                groupDto.CustomProperties = _groupCustomProperties.Select(x => new { x.Key, x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                var conversationDto = new ConversationDto
                {
                    ConversationID = unreadCountAndLastMessageByGroupId.GroupId,
                    GroupProfile = groupDto,
                    UnreadCount = unreadCountAndLastMessageByGroupId.UnreadCount,
                    LastMessageSentTime = unreadCountAndLastMessageByGroupId.LastSentTime,
                    LastMessage = unreadCountAndLastMessageByGroupId.LastMessage
                };
                conversations.Add(conversationDto);
            }

            return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Count };
        }

        public async Task<GetConversationProfileResponse> GetConversationProfileByIdAsync(
            GetConversationProfileRequest request, CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.ConversationId,
                    cancellationToken);
            groupUser.CheckExist(request.UserId, request.ConversationId);

            var (_, count) = await _messageDataProvider.GetUnreadCountByGroupIdsAsync(request.UserId, new List<string> { request.ConversationId }, cancellationToken).ConfigureAwait(false);

            var conversationDto = await GetConversationDto(groupUser, cancellationToken);
            conversationDto.UnreadCount = count;

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
            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            var messageIds = messages.Select(x => x.Id);
            var messageCustomProperties = await _messageCustomPropertyDataProvider.GetPropertiesByMessageIds(messageIds, cancellationToken).ConfigureAwait(false);
            foreach (var messageDto in messageDtos)
            {
                var _messageCustomProperties = messageCustomProperties.Where(x => x.MessageId == messageDto.Id).ToList();
                messageDto.CustomProperties = _messageCustomProperties.Select(x => new { x.Key, x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            }

            return new GetMessageListResponse
            {
                Messages = messageDtos,
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
            if (request.SearchParms == null || !request.SearchParms.Any())
            {
                return await GetConversationListByUserIdAsync(new GetConversationListRequest
                {
                    UserId = request.UserId,
                    PageSettings = request.PageSettings,
                    GroupIds = request.GroupIds,
                    GroupType = request.GroupType,
                    IncludeGroupByGroupCustomProperties = request.IncludeGroupByGroupCustomProperties,
                    ExcludeGroupByGroupCustomProperties = request.ExcludeGroupByGroupCustomProperties
                });
            }
            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, request.GroupIds, request.GroupType, cancellationToken)).Select(x => x.GroupId).ToList();

            var includeGroupIdsByCustomProperties = await _groupDataProvider.GetGroupIdByIncludeCustomPropertiesAsync(groupIds, request.IncludeGroupByGroupCustomProperties, cancellationToken).ConfigureAwait(false);
            groupIds = groupIds.Where(x => includeGroupIdsByCustomProperties.Contains(x)).ToList();

            var excludeGroupIdsByCustomProperties = await _groupDataProvider.GetGroupIdByExcludeCustomPropertiesAsync(groupIds, request.ExcludeGroupByGroupCustomProperties, cancellationToken).ConfigureAwait(false);
            groupIds = groupIds.Where(x => !excludeGroupIdsByCustomProperties.Contains(x)).ToList();

            if (!groupIds.Any())
                return new PagedResult<ConversationDto> { Result = new List<ConversationDto>(), Total = 0 };

            var filterGroupIds = new List<string>();
            var conversations = new List<ConversationDto>();
            var _groupIds = await _groupDataProvider.GetGroupIdsByMessageKeywordAsync(groupIds, request.SearchParms, request.IsExactSearch, request.GroupType, cancellationToken);
            filterGroupIds.AddRange(_groupIds);

            if (!filterGroupIds.Any())
                return new PagedResult<ConversationDto> { Result = conversations, Total = 0 };
            else
                groupIds = groupIds.Where(x => filterGroupIds.Contains(x)).ToList();

            var unreadCountAndLastMessageByGroupIds = await _messageDataProvider.GetUnreadCountAndLastMessageByGroupIdsAsync(request.UserId,
                    groupIds,
                    request.PageSettings,
                    request.GroupType,
                    cancellationToken).ConfigureAwait(false);
            if (!unreadCountAndLastMessageByGroupIds.Any())
                return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Count };

            var groupIdResults = unreadCountAndLastMessageByGroupIds.Select(x => x.GroupId);
            var groups = (await _groupDataProvider.GetByIdsAsync(groupIdResults, null, cancellationToken)).Result;
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groupIdResults, cancellationToken).ConfigureAwait(false);
            foreach (var unreadCountAndLastMessageByGroupId in unreadCountAndLastMessageByGroupIds)
            {
                var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == unreadCountAndLastMessageByGroupId.GroupId).ToList();
                var group = groups.SingleOrDefault(x => x.Id == unreadCountAndLastMessageByGroupId.GroupId);
                var groupDto = _mapper.Map<GroupDto>(group);
                groupDto.CustomProperties = _groupCustomProperties.Select(x => new { x.Key, x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
                var conversationDto = new ConversationDto
                {
                    ConversationID = unreadCountAndLastMessageByGroupId.GroupId,
                    GroupProfile = groupDto,
                    UnreadCount = unreadCountAndLastMessageByGroupId.UnreadCount,
                    LastMessageSentTime = unreadCountAndLastMessageByGroupId.LastSentTime,
                    LastMessage = unreadCountAndLastMessageByGroupId.LastMessage
                };
                conversations.Add(conversationDto);
            }

            return new PagedResult<ConversationDto> { Result = conversations, Total = groupIds.Count };
        }
    }
}