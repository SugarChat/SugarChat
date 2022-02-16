using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Exceptions;
using SugarChat.Message;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System;
using SugarChat.Message.Responses.GroupUsers;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Dtos.GroupUsers;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly ISecurityManager _securityManager;

        public GroupUserService(IMapper mapper, IGroupDataProvider groupDataProvider,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            ISecurityManager securityManager)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _securityManager = securityManager;
        }


        public async Task<UserAddedToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken).ConfigureAwait(false);
            group.CheckExist(command.GroupId);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken).ConfigureAwait(false);
            groupUser.CheckNotExist();

            groupUser = _mapper.Map<GroupUser>(command);
            await _groupUserDataProvider.AddAsync(groupUser, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<UserAddedToGroupEvent>(command);
        }

        public async Task<UserRemovedFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command,
            CancellationToken cancellationToken = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken).ConfigureAwait(false);
            group.CheckExist(command.GroupId);
            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken).ConfigureAwait(false);
            groupUser.CheckExist(command.UserId, command.GroupId);
            await _groupUserDataProvider.RemoveAsync(groupUser, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<UserRemovedFromGroupEvent>(command);
        }

        public async Task<GetGroupMembersResponse> GetGroupMemberIdsAsync(GetGroupMembersRequest request,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false);
            group.CheckExist(request.GroupId);
            IEnumerable<string> memberIds =
                (await _groupUserDataProvider.GetByGroupIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false)).Select(
                    o => o.UserId);
            return new()
            {
                MemberIds = memberIds
            };
        }

        public async Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request,
            CancellationToken cancellationToken = default)
        {
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.GroupId,
                    cancellationToken).ConfigureAwait(false);
            groupUser.CheckExist(request.UserId, request.GroupId);

            var groupUsers = await _groupUserDataProvider.GetMembersByGroupIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false);
            var groupUserDtos = _mapper.Map<IEnumerable<GroupUserDto>>(groupUsers);

            var userIds = groupUsers.Select(x => x.UserId).ToList();
            var users = await _userDataProvider.GetListAsync(x => userIds.Contains(x.Id)).ConfigureAwait(false);
            foreach (var groupUserDto in groupUserDtos)
            {
                var user = users.SingleOrDefault(x => x.Id == groupUserDto.UserId);
                groupUserDto.AvatarUrl = user?.AvatarUrl;
                groupUserDto.DisplayName = user?.DisplayName;
            }

            return new GetMembersOfGroupResponse
            {
                Members = groupUserDtos
            };
        }

        public async Task<GroupMemberCustomFieldSetEvent> SetGroupMemberCustomPropertiesAsync(
            SetGroupMemberCustomFieldCommand command, CancellationToken cancellationToken = default)
        {
            CheckProperties(command.CustomProperties);
            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId,
                    cancellationToken).ConfigureAwait(false);
            groupUser.CheckExist(command.UserId, command.GroupId);
            groupUser.CustomProperties = command.CustomProperties;
            await _groupUserDataProvider.UpdateAsync(groupUser, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<GroupMemberCustomFieldSetEvent>(command);
        }

        public async Task<GroupJoinedEvent> JoinGroupAsync(JoinGroupCommand command,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken).ConfigureAwait(false);
            group.CheckExist(command.GroupId);

            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);

            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken).ConfigureAwait(false);
            groupUser.CheckNotExist();

            groupUser = _mapper.Map<GroupUser>(command);
            await _groupUserDataProvider.AddAsync(groupUser, cancellationToken).ConfigureAwait(false);

            return _mapper.Map<GroupJoinedEvent>(command);
        }

        public async Task<GroupQuittedEvent> QuitGroupAsync(QuitGroupCommand command,
            CancellationToken cancellationToken = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken);
            group.CheckExist(command.GroupId);

            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken);
            user.CheckExist(command.UserId);

            GroupUser groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken);
            groupUser.CheckExist(command.UserId, command.GroupId);
            groupUser.CheckIsNotOwner(command.UserId, command.GroupId);

            await _groupUserDataProvider.RemoveAsync(groupUser, cancellationToken);

            return _mapper.Map<GroupQuittedEvent>(command);
        }

        public async Task<GroupOwnerChangedEvent> ChangeGroupOwnerAsync(ChangeGroupOwnerCommand command,
            CancellationToken cancellationToken = default)
        {
            CheckNotSameGroupUser(command.NewOwnerId, command.OwnerId);

            var groupOwner =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.OwnerId, command.GroupId, cancellationToken);
            groupOwner.CheckIsOwner(command.OwnerId, command.GroupId);

            var newGroupOwner =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.NewOwnerId, command.GroupId,
                    cancellationToken);
            newGroupOwner.CheckExist(command.NewOwnerId, command.GroupId);

            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken);
            group.CheckExist(command.GroupId);

            groupOwner.Role = UserRole.Admin;
            newGroupOwner.Role = UserRole.Owner;

            await _groupUserDataProvider.UpdateRangeAsync(new List<GroupUser> { groupOwner, newGroupOwner },
                cancellationToken);

            return _mapper.Map<GroupOwnerChangedEvent>(command);
        }


        public async Task<GroupMemberAddedEvent> AddGroupMembersAsync(AddGroupMemberCommand command,
            CancellationToken cancellationToken = default)
        {
            var group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken);
            group.CheckExist(command.GroupId);

            if (!await _securityManager.IsSupperAdmin())
            {
                var admin = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.AdminId, command.GroupId, cancellationToken);
                admin.CheckIsAdmin(command.AdminId, command.GroupId);
                IEnumerable<User> users = await _userDataProvider.GetRangeByIdAsync(command.GroupUserIds, cancellationToken);
                if (users.Count() != command.GroupUserIds.Count())
                {
                    throw new BusinessWarningException(Prompt.NotAllUsersExists);
                }
            }

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByGroupIdAndUsersIdAsync(command.GroupId, command.GroupUserIds, cancellationToken);
            if (groupUsers.Any())
            {
                throw new BusinessWarningException(Prompt.SomeGroupUsersExist);
            }

            var needAddGroupUsers = new List<GroupUser>();
            foreach (var groupUserId in command.GroupUserIds)
            {
                needAddGroupUsers.Add(new GroupUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = groupUserId,
                    GroupId = command.GroupId,
                    Role = command.Role,
                    CreatedBy = command.CreatedBy
                });
            }
            await _groupUserDataProvider.AddRangeAsync(needAddGroupUsers, cancellationToken);

            return _mapper.Map<GroupMemberAddedEvent>(command);
        }

        public async Task<GroupMemberRemovedEvent> RemoveGroupMembersAsync(RemoveGroupMemberCommand command,
            CancellationToken cancellationToken = default)
        {
            var admin = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.AdminId, command.GroupId,
                cancellationToken);
            admin.CheckIsAdmin(command.AdminId, command.GroupId);

            IEnumerable<GroupUser> groupUsers =
                await _groupUserDataProvider.GetByGroupIdAndUsersIdAsync(command.GroupId, command.UserIdList,
                    cancellationToken);
            if (groupUsers.Count() != command.UserIdList.Count)
            {
                throw new BusinessWarningException(Prompt.NotAllGroupUsersExist);
            }

            if (groupUsers.Any(o => o.Role == UserRole.Owner))
            {
                throw new BusinessWarningException(Prompt.RemoveOwnerFromGroup);
            }

            if (admin.Role == UserRole.Admin && groupUsers.Any(o => o.Role == UserRole.Admin))
            {
                throw new BusinessWarningException(Prompt.RemoveAdminByAdmin);
            }

            await _groupUserDataProvider.RemoveRangeAsync(groupUsers, cancellationToken);

            return _mapper.Map<GroupMemberRemovedEvent>(command);
        }

        public async Task<MessageRemindTypeSetEvent> SetMessageRemindTypeAsync(SetMessageRemindTypeCommand command,
            CancellationToken cancellationToken = default)
        {
            var user = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId,
                cancellationToken);
            user.CheckExist(command.UserId, command.GroupId);

            user.MessageRemindType = command.MessageRemindType;
            await _groupUserDataProvider.UpdateAsync(user, cancellationToken);

            return _mapper.Map<MessageRemindTypeSetEvent>(command);
        }

        public async Task<GroupMemberRoleSetEvent> SetGroupMemberRoleAsync(SetGroupMemberRoleCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!await _securityManager.IsSupperAdmin())
            {
                if (command.Role == UserRole.Owner)
                {
                    throw new BusinessWarningException(Prompt.SetGroupOwner);
                }
                var owner = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.OwnerId, command.GroupId, cancellationToken);
                owner.CheckIsOwner(command.OwnerId, command.GroupId);
            }
            var member = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.MemberId, command.GroupId, cancellationToken);
            member.CheckExist(command.MemberId, command.GroupId);
            member.Role = command.Role;
            await _groupUserDataProvider.UpdateAsync(member, cancellationToken);

            return _mapper.Map<GroupMemberRoleSetEvent>(command);
        }


        private void CheckProperties(Dictionary<string, string> properties)
        {
            if (properties is null || !properties.Any())
            {
                throw new BusinessWarningException(Prompt.NoCustomProperty);
            }
        }


        private void CheckNotSameGroupUser(string newOwnerId, string ownerId)
        {
            if (newOwnerId == ownerId)
            {
                throw new BusinessWarningException(Prompt.SameGroupUser);
            }
        }

        public async Task<GetUserIdsByGroupIdsResponse> GetUsersByGroupIdsAsync(GetUserIdsByGroupIdsRequest request, CancellationToken cancellationToken = default)
        {
            var groupUsers = await _groupUserDataProvider.GetByGroupIdsAsync(request.GroupIds, cancellationToken).ConfigureAwait(false);

            var userIds = groupUsers.Select(x => x.UserId).Distinct();

            return new GetUserIdsByGroupIdsResponse { UserIds = userIds };
        }

        public async Task UpdateGroupUserDataAsync(UpdateGroupUserDataCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);

            var ids = command.GroupUsers.Select(x => x.Id).ToArray();
            var groupUsers = await _groupUserDataProvider.GetListByIdsAsync(ids, cancellationToken).ConfigureAwait(false);
            var groups = (await _groupDataProvider.GetByIdsAsync(groupUsers.Select(x => x.GroupId), null, cancellationToken).ConfigureAwait(false)).Result;
            foreach (var groupUser in groupUsers)
            {
                var group = groups.SingleOrDefault(x => x.Id == groupUser.GroupId);
                group.CheckExist(groupUser.GroupId);
            }
            var userIds = groupUsers.Select(x => x.UserId);
            var users = await _userDataProvider.GetListAsync(x => userIds.Contains(x.Id));
            foreach (var groupUserDto in command.GroupUsers)
            {
                var groupUser = groupUsers.FirstOrDefault(x => x.Id == groupUserDto.Id);
                if (groupUser != null)
                {
                    _mapper.Map(groupUserDto, groupUser);
                }
            }
            await _groupUserDataProvider.UpdateRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
        }
    }
}