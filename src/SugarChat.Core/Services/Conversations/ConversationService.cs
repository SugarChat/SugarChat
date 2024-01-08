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
using SugarChat.Core.Services.GroupCustomProperties;
using Serilog;
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
        private readonly IGroup2DataProvider _group2DataProvider;

        public ConversationService(
            IMapper mapper,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IConversationDataProvider conversationDataProvider,
            IGroupDataProvider groupDataProvider,
            IMessageDataProvider messageDataProvider,
            IGroup2DataProvider group2DataProvider)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _conversationDataProvider = conversationDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
            _group2DataProvider = group2DataProvider;
        }

        public async Task<PagedResult<ConversationDto>> GetConversationListByUserIdAsync(GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var result = await _conversationDataProvider.GetConversationListAsync(request.UserId,
                request.GroupIds,
                request.GroupType,
                request.SearchParams,
                null,
                request.PageSettings.PageNum,
                request.PageSettings.PageSize,
                0,
                cancellationToken).ConfigureAwait(false);

            return result;
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
            var messageDtos = _mapper.Map<IEnumerable<MessageDto>>(messages);
            var messageIds = messages.Select(x => x.Id);

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

            if (groupUser is null)
                return _mapper.Map<ConversationRemovedEvent>(command);

            await _groupUserDataProvider.RemoveAsync(groupUser, cancellationToken);

            return _mapper.Map<ConversationRemovedEvent>(command);
        }

        public async Task RemoveConversationByConversationIdAsync2(RemoveConversationCommand command, CancellationToken cancellationToken = default)
        {
            var group = await _group2DataProvider.GetByIdAsync(command.ConversationId, cancellationToken);
            var groupUser = group.GroupUsers.FirstOrDefault(x => x.UserId == command.UserId);
            if (groupUser is not null)
            {
                group.GroupUsers.Remove(groupUser);
            }
            await _group2DataProvider.UpdateAsync(group, cancellationToken).ConfigureAwait(false);
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

        public async Task<PagedResult<ConversationDto>> GetConversationByKeywordAsync(GetConversationByKeywordRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var result = await _conversationDataProvider.GetConversationListAsync(request.UserId,
                request.GroupIds,
                request.GroupType,
                request.SearchParams,
                request.SearchMessageParams,
                request.PageSettings.PageNum,
                request.PageSettings.PageSize,
                request.MonthsAgo,
                cancellationToken).ConfigureAwait(false);

            return result;
        }

        public async Task<PagedResult<ConversationDto>> GetUnreadConversationListByUserIdAsync(GetUnreadConversationListRequest request, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var result = await _conversationDataProvider.GetUnreadConversationListAsync(request.UserId,
                request.GroupIds,
                request.GroupType,
                request.SearchParams,
                null,
                request.PageSettings.PageNum,
                request.PageSettings.PageSize,
                0,
                cancellationToken).ConfigureAwait(false);

            return result;
        }
    }
}