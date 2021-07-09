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
        private readonly ISecurityManager _securityManager;

        public ConversationService(
            IMapper mapper,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IConversationDataProvider conversationDataProvider,
            IGroupDataProvider groupDataProvider,
            IMessageDataProvider messageDataProvider,
            ISecurityManager securityManager)
        {
            _mapper = mapper;
            _userDataProvider = userDataProvider;
            _conversationDataProvider = conversationDataProvider;
            _groupDataProvider = groupDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
            _securityManager = securityManager;
        }

        public async Task<PagedResult<ConversationDto>> GetConversationListByUserIdAsync(GetConversationListRequest request, CancellationToken cancellationToken = default)
        {
            if (!await _securityManager.IsSupperAdmin())
            {
                var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
                user.CheckExist(request.UserId);
            }

            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId)).Select(x => x.GroupId).ToArray();
            var messages = await _messageDataProvider.GetByGroupIdsAsync(groupIds, cancellationToken);
            var groupUsers = await _groupUserDataProvider.GetGroupMemberCountByGroupIdsAsync(groupIds, cancellationToken);

            var groups = await _groupDataProvider.GetByIdsAsync(groupIds, request.PageSettings);
            var groupsResult = groups.Result;

            var groupsUnreadCountResult = (await _messageDataProvider.GetUserUnreadMessagesByGroupIdsAsync(request.UserId, groupsResult.Select(x => x.Id), cancellationToken))
                                 .GroupBy(x => x.GroupId).Select(x => new { GroupId = x.Key, UnreadCount = x.Count() });


            var conversations = new List<ConversationDto>();

            foreach (var group in groups.Result)
            {
                //Get the groups that have had conversations
                var groupMessages = messages.Where(x => x.GroupId == group.Id).OrderByDescending(x => x.SentTime);

                if (groupMessages.Any())
                {
                    var conversationDto = new ConversationDto();
                    conversationDto.ConversationID = group.Id;
                    conversationDto.UnreadCount = groupsUnreadCountResult.FirstOrDefault(x => x.GroupId == group.Id)?.UnreadCount ?? 0;
                    conversationDto.LastMessage = _mapper.Map<MessageDto>(groupMessages.FirstOrDefault());
                    var groupDto = _mapper.Map<GroupDto>(group);
                    groupDto.MemberCount = groupUsers.Where(x => x.GroupId == group.Id).Count();
                    conversationDto.GroupProfile = groupDto;
                    conversations.Add(conversationDto);
                }
            }

            return new PagedResult<ConversationDto> { Result = conversations, Total = groups.Total };
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
            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId)).Select(x => x.GroupId).ToArray();
            var messages = await _messageDataProvider.GetByGroupIdsAsync(groupIds, cancellationToken);
            var groupUsers = await _groupUserDataProvider.GetGroupMemberCountByGroupIdsAsync(groupIds, cancellationToken);

            List<string> filterGroupIds = new();
            if (request.SearchParms is not null && request.SearchParms.Count() != 0)
            {
                foreach (var searchParm in request.SearchParms)
                {
                    foreach (var message in messages)
                    {
                        dynamic payload = message.Payload;
                        IDictionary<string, object> dictionary = payload;
                        foreach (var item in dictionary)
                        {
                            if (item.Key.ToLower().Contains(searchParm.Key.ToLower()))
                            {
                                if ((request.IsExactSearch && item.Value.ToString() == searchParm.Value) || (!request.IsExactSearch && item.Value.ToString().Contains(searchParm.Value)))
                                {
                                    filterGroupIds.Add(message.GroupId);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                filterGroupIds = messages.Select(x => x.GroupId).ToList();
            }

            var groups = await _groupDataProvider.GetByIdsAsync(filterGroupIds, request.PageSettings);
            var groupsResult = groups.Result;

            var groupsUnreadCountResult = (await _messageDataProvider.GetUserUnreadMessagesByGroupIdsAsync(request.UserId, groupsResult.Select(x => x.Id), cancellationToken))
                                 .GroupBy(x => x.GroupId).Select(x => new { GroupId = x.Key, UnreadCount = x.Count() });

            var conversationDtos = new List<ConversationDto>();
            foreach (var group in groupsResult)
            {
                var lastMessage = messages.Where(x => x.GroupId == group.Id).OrderByDescending(x => x.SentTime).FirstOrDefault();
                var conversationDto = new ConversationDto();
                conversationDto.ConversationID = group.Id;
                conversationDto.UnreadCount = groupsUnreadCountResult.FirstOrDefault(x => x.GroupId == group.Id)?.UnreadCount ?? 0;
                conversationDto.LastMessage = _mapper.Map<MessageDto>(lastMessage);
                var groupDto = _mapper.Map<GroupDto>(group);
                groupDto.MemberCount = groupUsers.Where(x => x.GroupId == group.Id).Count();
                conversationDto.GroupProfile = groupDto;
                conversationDtos.Add(conversationDto);
            }
            return new PagedResult<ConversationDto> { Result = conversationDtos, Total = groups.Total };
        }
    }
}