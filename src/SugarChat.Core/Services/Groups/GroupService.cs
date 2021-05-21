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
using SugarChat.Core.IRepositories;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IRepository _repository;

        public GroupService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider, IRepository repository)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _repository = repository;
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

        public async Task DismissGroup(DismissGroupCommand command, CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation);
            group.CheckExist(command.Id);
            group.IsDel = true;
            await _groupDataProvider.UpdateAsync(group, cancellation);
        }

        public async Task JoinGroup(JoinGroupCommand command, CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellation);
            group.CheckNotExist();
            User user = await GetUserAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);
            if (await _repository.AnyAsync<GroupUser>(x => x.GroupId == command.GroupId && x.UserId == command.UserId))
            {
                throw new System.Exception("user has joined the group");
            }
            await _repository.AddAsync(new GroupUser
            {
                GroupId = command.GroupId,
                UserId = command.UserId
            });
        }
    }
}