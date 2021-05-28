using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupUserService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }

        public async Task<GroupJoinedEvent> JoinGroup(JoinGroupCommand command, CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellation);
            group.CheckExist(command.GroupId);

            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);

            GroupUser groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellation);
            groupUser.CheckNotExist(command.UserId, command.GroupId);

            await _groupUserDataProvider.AddAsync(new GroupUser
            {
                GroupId = command.GroupId,
                UserId = command.UserId
            }, cancellation);

            return new GroupJoinedEvent { };
        }

        public async Task<GroupQuittedEvent> QuitGroup(QuitGroupCommand command, CancellationToken cancellation)
        {
            GroupUser groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellation);
            groupUser.CheckExist(command.UserId, command.GroupId);
            groupUser.CheckIsNotOwner(command.UserId, command.GroupId);

            await _groupUserDataProvider.RemoveAsync(groupUser, cancellation);

            return new GroupQuittedEvent { };
        }

        public async Task<GroupOwnerChangedEvent> ChangeGroupOwner(ChangeGroupOwnerCommand command, CancellationToken cancellation)
        {
            var groupOwner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.FromUserId, command.GroupId, cancellation);
            groupOwner.CheckIsOwner(command.FromUserId, command.GroupId);

            var newGroupOwner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.ToUserId, command.GroupId, cancellation);
            newGroupOwner.CheckExist(command.ToUserId, command.GroupId);

            groupOwner.IsMaster = false;
            await _groupUserDataProvider.UpdateAsync(groupOwner, cancellation);

            newGroupOwner.IsMaster = true;
            newGroupOwner.IsAdmin = true;
            await _groupUserDataProvider.UpdateAsync(newGroupOwner, cancellation);

            return new GroupOwnerChangedEvent { };
        }

        public async Task<GroupMemberAddedEvent> AddGroupMember(AddGroupMemberCommand command, CancellationToken cancellationToken)
        {
            var admin = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.GroupAdminId, command.GroupId, cancellationToken);
            admin.CheckIsAdmin(command.GroupAdminId, command.GroupId);

            var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.MemberId, command.GroupId, cancellationToken);
            member.CheckNotExist(command.MemberId, command.GroupId);

            await _groupUserDataProvider.AddAsync(new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = command.MemberId,
                GroupId = command.GroupId
            }, cancellationToken);

            return new GroupMemberAddedEvent { };
        }
    }
}
