using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message;
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

            return _mapper.Map<GroupJoinedEvent>(command);
        }

        public async Task<GroupQuittedEvent> QuitGroup(QuitGroupCommand command, CancellationToken cancellation)
        {
            GroupUser groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellation);
            groupUser.CheckExist(command.UserId, command.GroupId);
            groupUser.CheckIsNotOwner(command.UserId, command.GroupId);

            await _groupUserDataProvider.RemoveAsync(groupUser, cancellation);

            return _mapper.Map<GroupQuittedEvent>(command);
        }

        public async Task<GroupOwnerChangedEvent> ChangeGroupOwner(ChangeGroupOwnerCommand command, CancellationToken cancellation)
        {
            var groupOwner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.FromUserId, command.GroupId, cancellation);
            groupOwner.CheckIsOwner(command.FromUserId, command.GroupId);

            var newGroupOwner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.ToUserId, command.GroupId, cancellation);
            newGroupOwner.CheckExist(command.ToUserId, command.GroupId);

            groupOwner.Role = UserRole.Admin;
            await _groupUserDataProvider.UpdateAsync(groupOwner, cancellation);

            newGroupOwner.Role = UserRole.Owner;
            await _groupUserDataProvider.UpdateAsync(newGroupOwner, cancellation);

            return _mapper.Map<GroupOwnerChangedEvent>(command);
        }

        public async Task<GroupMemberAddedEvent> AddGroupMember(AddGroupMemberCommand command, CancellationToken cancellationToken)
        {
            var admin = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.AdminId, command.GroupId, cancellationToken);
            admin.CheckIsAdmin(command.AdminId, command.GroupId);

            var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.MemberId, command.GroupId, cancellationToken);
            member.CheckNotExist(command.MemberId, command.GroupId);

            await _groupUserDataProvider.AddAsync(new GroupUser
            {
                Id = Guid.NewGuid().ToString(),
                UserId = command.MemberId,
                GroupId = command.GroupId
            }, cancellationToken);

            return _mapper.Map<GroupMemberAddedEvent>(command);
        }

        public async Task<GroupMemberDeletedEvent> DeleteGroupMember(DeleteGroupMemberCommand command, CancellationToken cancellationToken)
        {
            var admin = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.AdminId, command.GroupId, cancellationToken);
            admin.CheckIsAdmin(command.AdminId, command.GroupId);

            var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.MemberId, command.GroupId, cancellationToken);
            member.CheckExist(command.MemberId, command.GroupId);

            if (member.Role == UserRole.Owner)
            {
                throw new BusinessWarningException("the deleted member cannot be owner.");
            }

            if (admin.Role == UserRole.Admin && member.Role == UserRole.Admin)
            {
                throw new BusinessWarningException("amdin can't delete amdin.");
            }

            await _groupUserDataProvider.RemoveAsync(member, cancellationToken);

            return _mapper.Map<GroupMemberDeletedEvent>(command);
        }

        public async Task<MessageRemindTypeSetEvent> SetMessageRemindType(SetMessageRemindTypeCommand command, CancellationToken cancellationToken)
        {
            var user = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken);
            user.CheckExist(command.UserId, command.GroupId);

            user.MessageRemindType = command.MessageRemindType;
            await _groupUserDataProvider.UpdateAsync(user, cancellationToken);

            return _mapper.Map<MessageRemindTypeSetEvent>(command);
        }

        public async Task<GroupMemberRoleSetEvent> SetGroupMemberRole(SetGroupMemberRoleCommand command, CancellationToken cancellationToken)
        {
            if (command.Role == UserRole.Owner)
            {
                throw new BusinessWarningException("can't set group member role to owner.");
            }
            var owner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.OwnerId, command.GroupId, cancellationToken);
            owner.CheckIsOwner(command.OwnerId, command.GroupId);

            var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.MemberId, command.GroupId, cancellationToken);
            member.CheckExist(command.MemberId, command.GroupId);

            member.Role = command.Role;
            await _groupUserDataProvider.UpdateAsync(member, cancellationToken);

            return _mapper.Map<GroupMemberRoleSetEvent>(command);
        }
    }
}
