using AutoMapper;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Conversations;
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

        public async Task<GetConversationListByUserIdResponse> GetConversationListByUserIdAsync(GetConversationListByUserIdRequest request, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken);
            var conversations = new List<ConversationDto>();

            foreach (var groupUser in groupUsers)
            {
                //Get the groups that have had conversations
                var groupMessages = await _conversationDataProvider.GetMessagesByGroupIdAsync(groupUser.GroupId, cancellationToken);
                if (groupMessages != null && groupMessages.Count > 0)
                {
                    var conversationDto = new ConversationDto();
                    conversationDto.ConversationID = groupUser.GroupId;
                    conversationDto.UnreadCount = await _conversationDataProvider.GetUserUnreadMessagesCountByGroupIdAndLastReadTimeAsync(groupUser.GroupId, groupUser.LastReadTime, cancellationToken);
                    conversationDto.LastMessage = _mapper.Map<MessageDto>(await _conversationDataProvider.GetLastMessageByGroupIdAsync(groupUser.GroupId, cancellationToken));
                    conversationDto.GroupProfile = _mapper.Map<GroupDto>(await _groupDataProvider.GetByIdAsync(groupUser.GroupId, cancellationToken));
                    conversations.Add(conversationDto);
                }
            }

            return new GetConversationListByUserIdResponse
            {
                Result = conversations
            };
        }

        public async Task<GetConversationProfileByIdResponse> GetConversationProfileByIdRequestAsync(GetConversationProfileByIdRequest request, CancellationToken cancellationToken)
        {   
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var group = await _groupDataProvider.GetByIdAsync(request.ConversationId, cancellationToken);
            group.CheckExist(request.ConversationId);

            return new GetConversationProfileByIdResponse
            {
                Result = _mapper.Map<GroupDto>(group)
            };
        }

        //TODO:DeleteConversation
        public async Task DeleteConversationByIdAsync(DeleteConversationCommand command, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken);
            user.CheckExist(command.UserId);

            var group = await _groupDataProvider.GetByIdAsync(command.ConversationId, cancellationToken);
            group.CheckExist(command.ConversationId);

            //var groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.ConversationId, cancellationToken);
            
        }

        public async Task SetMessageReadByConversationIdAsync(SetMessageReadCommand command, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken);
            user.CheckExist(command.UserId);

            var group = await _groupDataProvider.GetByIdAsync(command.ConversationId, cancellationToken);
            group.CheckExist(command.ConversationId);

            var groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.ConversationId, cancellationToken);
            groupUser.CheckExist(command.UserId, command.ConversationId);

            groupUser.LastReadTime = DateTimeOffset.Now;
            await _groupUserDataProvider.UpdateAsync(groupUser,cancellationToken);
        }

    }
}
