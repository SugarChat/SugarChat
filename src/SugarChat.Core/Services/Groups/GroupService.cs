using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Messages;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IMessageDataProvider _messageDataProvider;

        public GroupService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider, IMessageDataProvider messageDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
        }

        public async Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command, CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation);
            group.CheckNotExist();

            group = _mapper.Map<Group>(command);
            await _groupDataProvider.AddAsync(group, cancellation).ConfigureAwait(false);

            return new GroupAddedEvent
            {
                Id = group.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(request.Id, cancellation);
            user.CheckExist(request.Id);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.Id, cancellation);
            IEnumerable<Group> groups =
                await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), cancellation);
            IEnumerable<GroupDto> groupsDto = _mapper.Map<IEnumerable<GroupDto>>(groups);
            return new()
            {
                Groups = groupsDto
            };
        }

        private Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return _userDataProvider.GetByIdAsync(id, cancellation);
        }

        public async Task<GetGroupProfileResponse> GetGroupProfileAsync(GetGroupProfileRequest request, CancellationToken cancellationToken)
        {
            var group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken);
            group.CheckExist(request.GroupId);

            var groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.GroupId, cancellationToken);
            groupUser.CheckExist(request.UserId, request.GroupId);

            var groupDto = _mapper.Map<GroupDto>(group);
            groupDto.MemberCount = await _groupUserDataProvider.GetGroupMemberCountAsync(request.GroupId, cancellationToken);

            return new GetGroupProfileResponse
            {
                Result = groupDto
            };
        }

        public async Task<GroupProfileUpdatedEvent> UpdateGroupProfileAsync(UpdateGroupProfileCommand command, CancellationToken cancellationToken)
        {           
            var group = await _groupDataProvider.GetByIdAsync(command.Id, cancellationToken);
            group.CheckExist(command.Id);

            group = _mapper.Map<Group>(command);
            await _groupDataProvider.UpdateAsync(group,cancellationToken);

            return _mapper.Map<GroupProfileUpdatedEvent>(command);
        }

        public async Task<GroupDismissedEvent> DismissGroup(DismissGroupCommand command, CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellation);
            group.CheckExist(command.GroupId);
            await _groupDataProvider.RemoveAsync(group, cancellation);

            var groupUsers = await _groupUserDataProvider.GetByGroupIdAsync(command.GroupId, cancellation);
            await _groupUserDataProvider.RemoveRangeAsync(groupUsers, cancellation);

            var messages = await _messageDataProvider.GetByGroupIdAsync(command.GroupId, cancellation);
            await _messageDataProvider.RemoveRangeAsync(messages, cancellation);

            return _mapper.Map<GroupDismissedEvent>(command);
        }
    }
}