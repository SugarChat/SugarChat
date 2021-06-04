using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System;
using System.Collections.Generic;
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

        public async Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request, CancellationToken cancellationToken)
        {
            var groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.GroupId, cancellationToken);
            groupUser.CheckExist(request.UserId, request.GroupId);

            var groupMembers = await _groupUserDataProvider.GetMembersByGroupIdAsync(request.GroupId, cancellationToken);

            return new GetMembersOfGroupResponse
            {
                Result = groupMembers
            };
        }

        public async Task<GroupMemberCustomFieldBeSetEvent> SetGroupMemberCustomFieldAsync(SetGroupMemberCustomFieldCommand command, CancellationToken cancellationToken)
        {
            var groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken);
            groupUser.CheckExist(command.UserId, command.GroupId);

            if (command.CustomProperties != null && command.CustomProperties.Count > 0)
            {
                groupUser.CustomProperties = command.CustomProperties;
                await _groupUserDataProvider.UpdateAsync(groupUser, cancellationToken);
                return _mapper.Map<GroupMemberCustomFieldBeSetEvent>(command);
            }
            else
            {
                throw new BusinessWarningException("Custom properties cannot be empty");
            }
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
            var groupOwner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.OwnerId, command.GroupId, cancellation);
            groupOwner.CheckIsOwner(command.OwnerId, command.GroupId);

            var newGroupOwner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.MewOwnerId, command.GroupId, cancellation);
            newGroupOwner.CheckExist(command.MewOwnerId, command.GroupId);

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

            List<GroupUser> groupUsers = new();
            foreach (var userId in command.UserIdList)
            {
                var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(userId, command.GroupId, cancellationToken);
                member.CheckNotExist(userId, command.GroupId);
                groupUsers.Add(new()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    GroupId = command.GroupId
                });
            }

            await _groupUserDataProvider.AddRangeAsync(groupUsers, cancellationToken);

            return _mapper.Map<GroupMemberAddedEvent>(command);
        }

        public async Task<GroupMemberDeletedEvent> DeleteGroupMember(DeleteGroupMemberCommand command, CancellationToken cancellationToken)
        {
            var admin = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.AdminId, command.GroupId, cancellationToken);
            admin.CheckIsAdmin(command.AdminId, command.GroupId);

            foreach (var userId in command.UserIdList)
            {
                var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(userId, command.GroupId, cancellationToken);
                member.CheckExist(userId, command.GroupId);

                if (member.Role == UserRole.Owner)
                {
                    throw new BusinessWarningException($"the deleted member cannot be owner with Id {member.UserId}.");
                }

                if (admin.Role == UserRole.Admin && member.Role == UserRole.Admin)
                {
                    throw new BusinessWarningException($"amdin can't delete amdin with Id {member.UserId}.");
                }
                await _groupUserDataProvider.RemoveAsync(member, cancellationToken);
            }

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
                throw new BusinessWarningException($"can't set group member role to owner with Id {command.MemberId}.");
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
