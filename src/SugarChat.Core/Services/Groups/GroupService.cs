using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Message.Dtos;
using SugarChat.Message.Paging;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Core.Services.Messages;
using SugarChat.Message;
using System;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.GroupCustomProperties;
using Serilog;
using SugarChat.Core.Utils;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IMessageDataProvider _messageDataProvider;
        private readonly ITransactionManager _transactionManagement;
        private readonly IGroupCustomPropertyDataProvider _groupCustomPropertyDataProvider;
        private readonly IGroup2DataProvider _group2DataProvider;

        public GroupService(IMapper mapper,
            IGroupDataProvider groupDataProvider,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IMessageDataProvider messageDataProvider,
            ITransactionManager transactionManagement,
            IGroupCustomPropertyDataProvider groupCustomPropertyDataProvider,
            IGroup2DataProvider group2DataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
            _transactionManagement = transactionManagement;
            _groupCustomPropertyDataProvider = groupCustomPropertyDataProvider;
            _group2DataProvider = group2DataProvider;
        }

        public async Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation).ConfigureAwait(false);
            group.CheckNotExist();

            group = _mapper.Map<Group>(command);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupId(command.Id, cancellation).ConfigureAwait(false);
            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
            {
                try
                {
                    await _groupDataProvider.AddAsync(group, cancellation).ConfigureAwait(false);
                    if (command.CustomProperties != null)
                    {
                        var _groupCustomProperties = new List<GroupCustomProperty>();
                        foreach (var customProperty in command.CustomProperties)
                        {
                            if (!groupCustomProperties.Any(x => x.Key == customProperty.Key && x.Value == customProperty.Value))
                            {
                                _groupCustomProperties.Add(new GroupCustomProperty
                                {
                                    GroupId = group.Id,
                                    Key = customProperty.Key,
                                    Value = customProperty.Value,
                                    CreatedBy = command.CreatedBy
                                });
                            }
                        }
                        await _groupCustomPropertyDataProvider.AddRangeAsync(_groupCustomProperties, cancellation).ConfigureAwait(false);
                    }
                }
                catch (MongoDB.Driver.MongoWriteException ex)
                {
                    await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    if (ex.WriteError.Code == 11000)
                    {
                        group.CheckNotExist();
                    }
                    throw;
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    throw;
                }
                try
                {
                    GroupUser groupUser = new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = command.UserId,
                        GroupId = command.Id,
                        Role = UserRole.Owner,
                        CreatedBy = command.CreatedBy,
                        GroupType = group.Type,
                        LastSentTime = group.LastSentTime,
                        CustomProperties = group.CustomProperties
                    };
                    await _groupUserDataProvider.AddAsync(groupUser, cancellation).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    throw;
                }
            }

            if (command.GroupUsers == null || !command.GroupUsers.Any())
                return _mapper.Map<GroupAddedEvent>(command);

            var time = 0;
            while (time < 3)
            {
                var groupUserIds = command.GroupUsers.Select(x => x.Id).ToList();
                var userIds = command.GroupUsers.Select(x => x.UserId).ToList();
                var users = await _userDataProvider.GetListAsync(x => userIds.Contains(x.Id), cancellation).ConfigureAwait(false);
                IEnumerable<string> existGroupUserIds = (await _groupUserDataProvider.GetByGroupIdAndUsersIdAsync(command.Id, groupUserIds, cancellation)).Select(x => x.UserId);
                var needAddGroupUserIds = groupUserIds.Where(x => !existGroupUserIds.Contains(x)).Distinct().ToList();
                if (!needAddGroupUserIds.Any())
                {
                    break;
                }

                var needAddGroupUsers = new List<GroupUser>();
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
                {
                    try
                    {
                        foreach (var groupUser in command.GroupUsers)
                        {
                            if (!users.Any(x => x.Id == groupUser.UserId))
                                continue;

                            if (groupUser.CustomProperties == null)
                                groupUser.CustomProperties = group.CustomProperties;
                            else
                            {
                                foreach (var customProperty in group.CustomProperties)
                                {
                                    if (!groupUser.CustomProperties.ContainsKey(customProperty.Key))
                                        groupUser.CustomProperties.Add(customProperty.Key, customProperty.Value);
                                }
                            }
                            needAddGroupUsers.Add(new GroupUser
                            {
                                Id = string.IsNullOrWhiteSpace(groupUser.Id) ? Guid.NewGuid().ToString() : groupUser.Id,
                                UserId = groupUser.UserId,
                                GroupId = command.Id,
                                Role = groupUser.Role,
                                GroupType = group.Type,
                                CustomProperties = groupUser.CustomProperties
                            });
                        }
                        await _groupUserDataProvider.AddRangeAsync(needAddGroupUsers, cancellation);
                        await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                        time++;
                        Log.Warning(ex, "AddGroupAsync.AddGroupMembers retry by " + time);
                        if (time > 2)
                        {
                            throw;
                        }
                    }
                }
            }
            return _mapper.Map<GroupAddedEvent>(command);
        }

        public async Task AddGroupAsync2(AddGroupCommand command, CancellationToken cancellation = default)
        {
            try
            {
                Group2 group = await _group2DataProvider.GetByIdAsync(command.Id, cancellation).ConfigureAwait(false);
                if (group is not null)
                    return;
                group = _mapper.Map<Group2>(command);
                group.GroupUsers.Add(new GroupUser2
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = command.UserId,
                    Role = UserRole.Owner,
                    CreatedBy = command.CreatedBy,
                });
                await _group2DataProvider.AddAsync(group, cancellation).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "AddGroupAsync2");
                throw;
            }
        }

        public async Task BatchAddGroupAsync(BatchAddGroupCommand command, CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);

            var groupIds = command.AddGroupCommands.Select(x => x.Id).ToList();

            int retryCount = 1;
            while (retryCount <= 3)
            {
                var existGroupIds = (await _groupDataProvider.GetListAsync(x => groupIds.Contains(x.Id), cancellation).ConfigureAwait(false))
                    .Select(x => x.Id).ToList();
                var groups = _mapper.Map<List<Group>>(command.AddGroupCommands.Where(x => !existGroupIds.Contains(x.Id)).ToList());
                var groupCustomProperties = await _groupCustomPropertyDataProvider.GetListAsync(x => groupIds.Contains(x.Id), cancellation).ConfigureAwait(false);
                var newGroupCustomProperties = new List<GroupCustomProperty>();
                var groupUsers = new List<GroupUser>();

                foreach (var group in groups)
                {
                    foreach (var customProperty in group.CustomProperties)
                    {
                        if (!groupCustomProperties.Any(x => x.Key == customProperty.Key && x.Value == customProperty.Value))
                        {
                            newGroupCustomProperties.Add(new GroupCustomProperty
                            {
                                GroupId = group.Id,
                                Key = customProperty.Key,
                                Value = customProperty.Value,
                                CreatedBy = command.UserId
                            });
                        }
                    }

                    groupUsers.Add(new GroupUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = command.UserId,
                        GroupId = group.Id,
                        Role = UserRole.Owner,
                        CreatedBy = command.UserId,
                        GroupType = group.Type,
                        LastSentTime = group.LastSentTime,
                        CustomProperties = group.CustomProperties
                    });
                }
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
                {
                    try
                    {
                        await _groupDataProvider.AddRangeAsync(groups, cancellation).ConfigureAwait(false);
                        await _groupCustomPropertyDataProvider.AddRangeAsync(newGroupCustomProperties, cancellation).ConfigureAwait(false);
                        await _groupUserDataProvider.AddRangeAsync(groupUsers, cancellation).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                        break;
                    }
                    catch (Exception)
                    {
                        await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                        if (retryCount >= 3)
                            throw;
                        retryCount++;
                    }
                }
            }
        }

        public async Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(request.UserId, cancellation).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, null, request.GroupType, cancellation).ConfigureAwait(false);
            if (!groupUsers.Any())
                return new()
                {
                    Groups = new()
                    {
                        Result = new List<GroupDto>(),
                        Total = 0
                    }
                };

            PagedResult<Group> groups = await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), request.PageSettings, cancellation).ConfigureAwait(false);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groups.Result.Select(x => x.Id)).ConfigureAwait(false);
            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups.Result);
            foreach (var groupDto in groupDtos)
            {
                var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == groupDto.Id).ToList();
                groupDto.CustomProperties = _groupCustomProperties.GroupBy(x => x.Key).Select(x => x.OrderByDescending(y => y.CreatedBy).First()).ToDictionary(x => x.Key, x => x.Value);

                if (groupDto.CustomProperties.Count < _groupCustomProperties.Count)
                    Log.Warning("GetGroupsOfUserAsync: An item with the same key has already been added.GroupId: " + groupDto.Id);
            }

            PagedResult<GroupDto> groupsDto = new()
            {
                Result = groupDtos,
                Total = groups.Total
            };
            return new()
            {
                Groups = groupsDto
            };
        }

        public async Task<GroupRemovedEvent> RemoveGroupAsync(RemoveGroupCommand command,
            CancellationToken cancellation = default)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation).ConfigureAwait(false);
            group.CheckExist(command.Id);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupId(group.Id);

            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
            {
                try
                {
                    await _groupDataProvider.RemoveAsync(group, cancellation).ConfigureAwait(false);
                    await _groupCustomPropertyDataProvider.RemoveRangeAsync(groupCustomProperties, cancellation).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    throw;
                }
            }
            return _mapper.Map<GroupRemovedEvent>(command);
        }

        public async Task<GetGroupProfileResponse> GetGroupProfileAsync(GetGroupProfileRequest request,
            CancellationToken cancellationToken = default)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            var group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false);
            if (group is null)
            {
                return new GetGroupProfileResponse
                {
                    Group = null
                };
            }

            var groupDto = _mapper.Map<GroupDto>(group);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupId(group.Id);
            groupDto.CustomProperties = groupCustomProperties.GroupBy(x => x.Key).Select(x => x.OrderByDescending(y => y.CreatedBy).First()).ToDictionary(x => x.Key, x => x.Value);
            groupDto.MemberCount =
                await _groupUserDataProvider.GetGroupMemberCountByGroupIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false);

            if (groupDto.CustomProperties.Count < groupCustomProperties.Count())
                Log.Warning("GetGroupProfileAsync: An item with the same key has already been added.GroupId: " + groupDto.Id);

            return new GetGroupProfileResponse
            {
                Group = groupDto
            };
        }

        public async Task<GroupProfileUpdatedEvent> UpdateGroupProfileAsync(UpdateGroupProfileCommand command,
            CancellationToken cancellationToken)
        {
            var group = await _groupDataProvider.GetByIdAsync(command.Id, cancellationToken).ConfigureAwait(false);
            group.CheckExist(command.Id);

            _mapper.MapIgnoreNull(command, group);

            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupId(command.Id, cancellationToken).ConfigureAwait(false);
            var newGroupCustomProperties = new List<GroupCustomProperty>();
            if (command.CustomProperties != null)
            {
                foreach (var customProperty in command.CustomProperties)
                {
                    newGroupCustomProperties.Add(new GroupCustomProperty
                    {
                        GroupId = group.Id,
                        Key = customProperty.Key,
                        Value = customProperty.Value,
                        CreatedBy = group.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    });
                }
            }
            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    await _groupDataProvider.UpdateAsync(group, cancellationToken).ConfigureAwait(false);
                    await _groupCustomPropertyDataProvider.RemoveRangeAsync(groupCustomProperties, cancellationToken).ConfigureAwait(false);
                    await _groupCustomPropertyDataProvider.AddRangeAsync(newGroupCustomProperties, cancellationToken).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    throw;
                }
            }

            return _mapper.Map<GroupProfileUpdatedEvent>(command);
        }

        public async Task<GroupDismissedEvent> DismissGroupAsync(DismissGroupCommand command, CancellationToken cancellation)
        {
            Group group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellation).ConfigureAwait(false);
            group.CheckExist(command.GroupId);

            var messages = await _messageDataProvider.GetByGroupIdAsync(command.GroupId, cancellation).ConfigureAwait(false);
            var groupUsers = await _groupUserDataProvider.GetByGroupIdAsync(command.GroupId, cancellation).ConfigureAwait(false);

            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
            {
                try
                {
                    await _messageDataProvider.RemoveRangeAsync(messages, cancellation).ConfigureAwait(false);
                    await _groupUserDataProvider.RemoveRangeAsync(groupUsers, cancellation).ConfigureAwait(false);
                    await _groupDataProvider.RemoveAsync(group, cancellation).ConfigureAwait(false);
                    await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    throw;
                }
            }

            return _mapper.Map<GroupDismissedEvent>(command);
        }

        public async Task<IEnumerable<GroupDto>> GetByCustomProperties(GetGroupByCustomPropertiesRequest request, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            List<string> groupIds = new List<string>();
            if (!request.SearchAllGroup)
            {
                groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, null, request.GroupType, cancellationToken).ConfigureAwait(false)).Select(x => x.GroupId).ToList();
                if (!groupIds.Any())
                {
                    return new GroupDto[] { };
                }
            }
            var groups = await _groupDataProvider.GetByCustomPropertiesAsync(groupIds, request.CustomProperties, null, cancellationToken).ConfigureAwait(false);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groups.Select(x => x.Id));

            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups);
            foreach (var groupDto in groupDtos)
            {
                var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == groupDto.Id).ToList();
                groupDto.CustomProperties = _groupCustomProperties.GroupBy(x => x.Key).Select(x => x.OrderByDescending(y => y.CreatedBy).First()).ToDictionary(x => x.Key, x => x.Value);
                if (groupDto.CustomProperties.Count < _groupCustomProperties.Count())
                    Log.Warning("GetGroupProfileAsync: An item with the same key has already been added.GroupId: " + groupDto.Id);
            }

            return groupDtos;
        }

        /// <summary>
        /// 迁移数据使用，一次性代码
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        public async Task MigrateCustomPropertyAsync(CancellationToken cancellation = default)
        {
            var total = await _groupDataProvider.GetCountAsync(x => x.CustomProperties != null && x.CustomProperties != new Dictionary<string, string>(), cancellation).ConfigureAwait(false);
            var pageSize = 500;
            var pageIndex = total / pageSize + 1;
            for (int i = 1; i <= pageIndex; i++)
            {
                using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
                {
                    try
                    {
                        var groups = await _groupDataProvider.GetListAsync(new PageSettings { PageNum = 1, PageSize = pageSize }, x => x.CustomProperties != null && x.CustomProperties != new Dictionary<string, string>(), cancellation).ConfigureAwait(false);
                        var groupCustomProperties = new List<GroupCustomProperty>();
                        foreach (var group in groups)
                        {
                            foreach (var customProperty in group.CustomProperties)
                            {
                                groupCustomProperties.Add(new GroupCustomProperty { GroupId = group.Id, Key = customProperty.Key, Value = customProperty.Value });
                            }
                            group.CustomProperties = new Dictionary<string, string>();
                        }
                        await _groupDataProvider.UpdateRangeAsync(groups, cancellation).ConfigureAwait(false);
                        await _groupCustomPropertyDataProvider.AddRangeAsync(groupCustomProperties, cancellation).ConfigureAwait(false);
                        await transaction.CommitAsync(cancellation).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Migrate Group CustomProperty Error");
                        await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
                    }
                }
            }
        }
    }
}