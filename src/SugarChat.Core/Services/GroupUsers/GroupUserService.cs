using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Event;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupUserService(IMapper mapper, IGroupDataProvider groupDataProvider,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }


        public async Task<AddUserToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command,
            CancellationToken cancellation)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellation);
            group.CheckExist(command.GroupId);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellation);
            groupUser.CheckNotExist();

            groupUser = _mapper.Map<GroupUser>(command);
            await _groupUserDataProvider.AddAsync(groupUser, cancellation);
            return new AddUserToGroupEvent
            {
                Id = groupUser.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<RemoveUserFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command,
            CancellationToken cancellation)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellation);
            group.CheckExist(command.GroupId);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellation);
            groupUser.CheckExist(command.UserId, command.GroupId);
            await _groupUserDataProvider.RemoveAsync(groupUser, cancellation);
            return new RemoveUserFromGroupEvent
            {
                Id = groupUser.Id,
                Status = EventStatus.Success
            };
        }

        public async Task<GetGroupMembersResponse> GetGroupMemberIdsAsync(GetGroupMembersRequest request,
            CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellation);
            group.CheckExist(request.GroupId);
            IEnumerable<string> memberIds =
                (await _groupUserDataProvider.GetByGroupIdAsync(request.GroupId, cancellation)).Select(
                    o => o.UserId);
            return new()
            {
                MemberIds = memberIds
            };
        }
    }
}