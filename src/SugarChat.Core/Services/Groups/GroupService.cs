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
using SugarChat.Shared.Paging;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Requests.Groups;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }

        public async Task<AddGroupEvent> AddGroupAsync(AddGroupCommand command,
            CancellationToken cancellation = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation);
            group.CheckNotExist();

            group = _mapper.Map<Group>(command);
            await _groupDataProvider.AddAsync(group, cancellation).ConfigureAwait(false);

            return new AddGroupEvent
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
            PagedResult<Group> groups =
                await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), request.PageSettings,
                    cancellation);
            PagedResult<GroupDto> groupsDto = new()
            {
                Result = _mapper.Map<IEnumerable<GroupDto>>(groups.Result),
                Total = groups.Total
            };
            return new()
            {
                Groups = groupsDto
            };
        }

        public async Task<RemoveGroupEvent> RemoveGroupAsync(RemoveGroupCommand command,
            CancellationToken cancellation = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation);
            group.CheckExist(command.Id);
            await _groupDataProvider.RemoveAsync(group, cancellation);
            return new RemoveGroupEvent {Id = group.Id, Status = EventStatus.Success};
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
    }
}