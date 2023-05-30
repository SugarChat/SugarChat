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
using SugarChat.Core.Services.GroupUserCustomProperties;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Paging;
using Serilog;
using SugarChat.Core.Services.GroupCustomProperties;
using SugarChat.Core.Utils;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly ISecurityManager _securityManager;
        private readonly ITransactionManager _transactionManagement;
        private readonly IGroupUserCustomPropertyDataProvider _groupUserCustomPropertyDataProvider;
        private readonly IGroupCustomPropertyDataProvider _groupCustomPropertyDataProvider;

        public GroupUserService(IMapper mapper, IGroupDataProvider groupDataProvider,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            ISecurityManager securityManager,
            ITransactionManager transactionManagement,
            IGroupUserCustomPropertyDataProvider groupUserCustomPropertyDataProvider,
            IGroupCustomPropertyDataProvider groupCustomPropertyDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _securityManager = securityManager;
            _transactionManagement = transactionManagement;
            _groupUserCustomPropertyDataProvider = groupUserCustomPropertyDataProvider;
            _groupCustomPropertyDataProvider = groupCustomPropertyDataProvider;
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
            groupUser.GroupType = group.Type;
            groupUser.LastSentTime = group.LastSentTime;
            groupUser.CustomProperties = group.CustomProperties;
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
            var groupUserCustomProperties = await _groupUserCustomPropertyDataProvider.GetPropertiesByGroupUserId(groupUser.Id, cancellationToken).ConfigureAwait(false);

            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _groupUserCustomPropertyDataProvider.RemoveRangeAsync(groupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _groupUserDataProvider.RemoveAsync(groupUser, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }

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
            var groupUserCustomProperties = await _groupUserCustomPropertyDataProvider.GetPropertiesByGroupUserIds(groupUsers.Select(x => x.Id), cancellationToken).ConfigureAwait(false);
            foreach (var groupUserDto in groupUserDtos)
            {
                var user = users.SingleOrDefault(x => x.Id == groupUserDto.UserId);
                groupUserDto.AvatarUrl = user?.AvatarUrl;
                groupUserDto.DisplayName = user?.DisplayName;
                var _groupUserCustomProperties = groupUserCustomProperties.Where(x => x.GroupUserId == groupUserDto.Id).ToList();
                groupUserDto.CustomProperties = _groupUserCustomProperties.GroupBy(x => x.Key).Select(x => x.OrderByDescending(y => y.CreatedBy).First()).ToDictionary(x => x.Key, x => x.Value);
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

            var groupUser_CustomProperties = new Dictionary<string, string>();
            var newGroupUserCustomProperties = new List<GroupUserCustomProperty>();
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupId(command.GroupId, cancellationToken).ConfigureAwait(false);
            foreach (var customProperty in groupCustomProperties)
            {
                newGroupUserCustomProperties.Add(new GroupUserCustomProperty
                {
                    GroupUserId = groupUser.Id,
                    Key = customProperty.Key,
                    Value = customProperty.Value,
                    CreatedBy = command.UserId
                });
                if (!groupUser_CustomProperties.ContainsKey(customProperty.Key))
                    groupUser_CustomProperties.Add(customProperty.Key, customProperty.Value);
            }

            var groupUserCustomProperties = await _groupUserCustomPropertyDataProvider.GetPropertiesByGroupUserId(groupUser.Id, cancellationToken).ConfigureAwait(false);
            if (command.CustomProperties != null && command.CustomProperties.Any())
            {
                foreach (var customProperty in command.CustomProperties)
                {
                    newGroupUserCustomProperties.Add(new GroupUserCustomProperty
                    {
                        GroupUserId = groupUser.Id,
                        Key = customProperty.Key,
                        Value = customProperty.Value,
                        CreatedBy = command.UserId
                    });
                    if (!groupUser_CustomProperties.ContainsKey(customProperty.Key))
                        groupUser_CustomProperties.Add(customProperty.Key, customProperty.Value);
                }
            }
            groupUser.CustomProperties = groupUser_CustomProperties;

            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _groupUserDataProvider.UpdateAsync(groupUser, cancellationToken).ConfigureAwait(false);
                    await _groupUserCustomPropertyDataProvider.RemoveRangeAsync(groupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _groupUserCustomPropertyDataProvider.AddRangeAsync(newGroupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }

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
            groupUser.GroupType = group.Type;
            groupUser.LastSentTime = group.LastSentTime;
            groupUser.CustomProperties = group.CustomProperties;
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

            var time = 0;
            while (time < 3)
            {
                IEnumerable<string> existGroupUserIds = (await _groupUserDataProvider.GetByGroupIdAndUsersIdAsync(command.GroupId, command.GroupUserIds, cancellationToken)).Select(x => x.UserId);
                var needAddGroupUserIds = command.GroupUserIds.Where(x => !existGroupUserIds.Contains(x)).Distinct().ToList();
                if (!needAddGroupUserIds.Any())
                {
                    break;
                }

                var needAddGroupUsers = new List<GroupUser>();
                var groupUserCustomProperties = new List<GroupUserCustomProperty>();
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        foreach (var groupUserId in needAddGroupUserIds)
                        {
                            var groupUser_CustomProperties = new Dictionary<string, string>();
                            if (group.CustomProperties != null)
                            {
                                foreach (var customProperty in group.CustomProperties)
                                {
                                    if (!groupUser_CustomProperties.ContainsKey(customProperty.Key))
                                        groupUser_CustomProperties.Add(customProperty.Key, customProperty.Value);
                                }
                            }
                            var groupUser = new GroupUser
                            {
                                Id = Guid.NewGuid().ToString(),
                                UserId = groupUserId,
                                GroupId = command.GroupId,
                                Role = command.Role,
                                CreatedBy = command.CreatedBy,
                                GroupType = group.Type,
                                LastSentTime = group.LastSentTime,
                                CustomProperties = groupUser_CustomProperties,
                            };
                            if (command.CustomProperties != null)
                            {
                                foreach (var customProperty in command.CustomProperties)
                                {
                                    groupUserCustomProperties.Add(new GroupUserCustomProperty
                                    {
                                        GroupUserId = groupUser.Id,
                                        Key = customProperty.Key,
                                        Value = customProperty.Value,
                                        CreatedBy = command.CreatedBy
                                    });
                                    if (!groupUser_CustomProperties.ContainsKey(customProperty.Key))
                                        groupUser_CustomProperties.Add(customProperty.Key, customProperty.Value);
                                }
                            }
                            needAddGroupUsers.Add(groupUser);
                        }
                        await _groupUserDataProvider.AddRangeAsync(needAddGroupUsers, cancellationToken);
                        await _groupUserCustomPropertyDataProvider.AddRangeAsync(groupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                        time++;
                        Log.Warning(ex, "AddGroupMembersAsync retry by " + time);
                        if (time > 2)
                        {
                            throw;
                        }
                    }
                }
            }

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
            if (!groupUsers.Any())
                return _mapper.Map<GroupMemberRemovedEvent>(command);

            if (groupUsers.Any(o => o.Role == UserRole.Owner))
            {
                throw new BusinessWarningException(Prompt.RemoveOwnerFromGroup);
            }
            if (admin.Role == UserRole.Admin && groupUsers.Any(o => o.Role == UserRole.Admin))
            {
                throw new BusinessWarningException(Prompt.RemoveAdminByAdmin);
            }
            var groupUserCustomProperties = await _groupUserCustomPropertyDataProvider.GetPropertiesByGroupUserIds(groupUsers.Select(x => x.Id), cancellationToken).ConfigureAwait(false);

            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _groupUserCustomPropertyDataProvider.RemoveRangeAsync(groupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _groupUserDataProvider.RemoveRangeAsync(groupUsers, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }

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

            var userIds = groupUsers.Select(x => x.UserId).Distinct().ToList();

            return new GetUserIdsByGroupIdsResponse { UserIds = userIds };
        }

        public async Task UpdateGroupUserDataAsync(UpdateGroupUserDataCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(command.UserId);

            var ids = command.GroupUsers.Select(x => x.Id).ToList();
            var groupUsers = await _groupUserDataProvider.GetListByIdsAsync(ids, cancellationToken).ConfigureAwait(false);
            var groupIds = groupUsers.Select(x => x.GroupId).ToList();
            groupIds.AddRange(command.GroupUsers.Select(x => x.GroupId).ToList());
            var groups = (await _groupDataProvider.GetByIdsAsync(groupIds, null, cancellationToken).ConfigureAwait(false)).Result;
            foreach (var groupUser in groupUsers)
            {
                var group = groups.SingleOrDefault(x => x.Id == groupUser.GroupId);
                group.CheckExist(groupUser.GroupId);
            }

            var newGroupUsers = _mapper.Map<IEnumerable<GroupUser>>(command.GroupUsers);
            var groupUserCustomProperties = await _groupUserCustomPropertyDataProvider.GetPropertiesByGroupUserIds(command.GroupUsers.Select(x => x.Id), cancellationToken).ConfigureAwait(false);
            var oldGroupUserCustomProperties = new List<GroupUserCustomProperty>();
            var newGroupUserCustomProperties = new List<GroupUserCustomProperty>();

            foreach (var groupUser in newGroupUsers)
            {
                groupUser.LastModifyBy = command.UserId;
                groupUser.LastModifyDate = DateTime.UtcNow;
                if (groupUser.CustomProperties == null)
                    groupUser.CustomProperties = new Dictionary<string, string>();
                var group = groups.Single(x => x.Id == groupUser.GroupId);
                if (group.CustomProperties != null)
                {
                    foreach (var customProperty in group.CustomProperties)
                    {
                        if (!groupUser.CustomProperties.ContainsKey(customProperty.Key))
                            groupUser.CustomProperties.Add(customProperty.Key, customProperty.Value);
                    }
                }

                oldGroupUserCustomProperties.AddRange(groupUserCustomProperties.Where(x => x.GroupUserId == groupUser.Id).ToList());

                if (groupUser.CustomProperties != null && groupUser.CustomProperties.Any())
                {
                    foreach (var customProperty in groupUser.CustomProperties)
                    {
                        newGroupUserCustomProperties.Add(new GroupUserCustomProperty
                        {
                            GroupUserId = groupUser.Id,
                            Key = customProperty.Key,
                            Value = customProperty.Value,
                            CreatedBy = command.UserId
                        });
                    }
                }
            }

            var time = 0;
            while (time < 3)
            {
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        await _groupUserDataProvider.UpdateRangeAsync(newGroupUsers, groupUsers, cancellationToken).ConfigureAwait(false);
                        await _groupUserCustomPropertyDataProvider.RemoveRangeAsync(oldGroupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                        await _groupUserCustomPropertyDataProvider.AddRangeAsync(newGroupUserCustomProperties, cancellationToken).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                        time++;
                        Log.Warning(ex, "UpdateGroupUserDataAsync retry by " + time);
                        if (time > 2)
                        {
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 迁移数据使用，一次性代码
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task MigrateCustomPropertyAsync(CancellationToken cancellation = default)
        {
            var groupIds = _groupDataProvider.GetGroupIds(x => x.Type == 0);

            var total = await _groupUserDataProvider.GetCountAsync(x => groupIds.Contains(x.GroupId), cancellation).ConfigureAwait(false);
            var pageSize = 5000;
            var pageIndex = total / pageSize + 1;
            for (int i = 1; i <= pageIndex; i++)
            {
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
                {
                    try
                    {
                        var groupUsers = await _groupUserDataProvider.GetListAsync(new PageSettings { PageNum = i, PageSize = pageSize }, x => groupIds.Contains(x.GroupId), cancellation).ConfigureAwait(false);
                        var groupUserCustomProperties = new List<GroupUserCustomProperty>();
                        foreach (var groupUser in groupUsers)
                        {
                            groupUserCustomProperties.Add(new GroupUserCustomProperty { GroupUserId = groupUser.Id, Key = "UserType", Value = groupUser.Role == UserRole.Member ? "Customer" : "Merchant" });
                        }
                        await _groupUserCustomPropertyDataProvider.AddRangeAsync(groupUserCustomProperties, cancellation).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Migrate GroupUser CustomProperty Error");
                        await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    }
                }
            }
        }

        public async Task MigrateGroupCustomPropertyAsyncToGroupUser(int pageSize, CancellationToken cancellation = default)
        {
            Log.Warning("Migrate Group CustomProperty To GroupUser Start");
            var groups = await _groupDataProvider.GetListAsync();
            var total = groups.Count();
            var pageIndex = total / pageSize + 1;
            for (int i = 1; i <= pageIndex; i++)
            {
                Log.Warning("Migrate Group CustomProperty To GroupUser " + i);
                var updateGroupUsers = new List<GroupUser>();
                var groups1 = groups.OrderBy(x => x.Id).Skip((i - 1) * pageSize).Take(pageSize).ToList();
                for (int j = 1; j <= 10; j++)
                {
                    int pageSize2 = pageSize / 10;
                    var groups2 = groups1.OrderBy(x => x.Id).Skip((j - 1) * pageSize2).Take(pageSize2).ToList();
                    var groupIds = groups2.Select(x => x.Id).ToList();
                    var groupUsers = await _groupUserDataProvider.GetListAsync(x => groupIds.Contains(x.GroupId), cancellation).ConfigureAwait(false);
                    var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groupIds, cancellation).ConfigureAwait(false);

                    foreach (var groupId in groupIds)
                    {
                        var group = groups2.Single(x => x.Id == groupId);
                        var _groupUsers = groupUsers.Where(x => x.GroupId == groupId).ToList();
                        var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == groupId).ToList();
                        Dictionary<string, string> customProperties = new Dictionary<string, string>();
                        foreach (var groupCustomProperty in _groupCustomProperties)
                        {
                            customProperties.Add(groupCustomProperty.Key, groupCustomProperty.Value);
                        }
                        foreach (var groupUser in _groupUsers)
                        {
                            groupUser.GroupType = group.Type;
                            groupUser.LastSentTime = group.LastSentTime;
                            groupUser.CustomProperties = customProperties;
                            updateGroupUsers.Add(groupUser);
                        }
                        group.CustomProperties = customProperties;
                    }
                }
                try
                {
                    await _groupUserDataProvider.UpdateRangeAsync(updateGroupUsers, cancellation).ConfigureAwait(false);
                    await _groupDataProvider.UpdateRangeAsync(groups1, cancellation).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Migrate Group CustomProperty To GroupUser Error");
                }
            }

            Log.Warning("Migrate Group CustomProperty To GroupUser End");
        }

        public async Task<bool> CheckUserIsInGroupAsync(CheckUserIsInGroupCommand command, CancellationToken cancellation = default)
        {
            return (await _groupUserDataProvider.GetByGroupIdAndUsersIdAsync(command.GroupId, command.UserIds, cancellation).ConfigureAwait(false)).Any();
        }
    }
}