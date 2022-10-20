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
        public GroupService(IMapper mapper,
            IGroupDataProvider groupDataProvider,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider,
            IMessageDataProvider messageDataProvider,
            ITransactionManager transactionManagement,
            IGroupCustomPropertyDataProvider groupCustomPropertyDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
            _transactionManagement = transactionManagement;
            _groupCustomPropertyDataProvider = groupCustomPropertyDataProvider;
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
                    group.CustomProperties = null;
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
                        CreatedBy = command.CreatedBy
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
            return _mapper.Map<GroupAddedEvent>(command);
        }

        public async Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(request.UserId, cancellation).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, null, cancellation, request.GroupType).ConfigureAwait(false);
            PagedResult<Group> groups = await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), request.PageSettings, cancellation).ConfigureAwait(false);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groups.Result.Select(x => x.Id)).ConfigureAwait(false);
            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups.Result);
            foreach (var groupDto in groupDtos)
            {
                var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == groupDto.Id).ToList();
                groupDto.CustomPropertyList = _mapper.Map<IEnumerable<GroupCustomPropertyDto>>(_groupCustomProperties);
                groupDto.CustomProperties = _groupCustomProperties.Select(x => new { x.Key, x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);

                if (groupDto.CustomProperties.Count < _groupCustomProperties.Count)
                    Log.Warning("GetGroupsOfUserAsync: An item with the same key has already been added.");
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
            groupDto.CustomPropertyList = _mapper.Map<IEnumerable<GroupCustomPropertyDto>>(groupCustomProperties);
            groupDto.CustomProperties = groupCustomProperties.Select(x => new { x.Key, x.Value }).Distinct().ToDictionary(x => x.Key, x => x.Value);
            groupDto.MemberCount =
                await _groupUserDataProvider.GetGroupMemberCountByGroupIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false);

            if (groupDto.CustomProperties.Count < groupCustomProperties.Count())
                Log.Warning("GetGroupProfileAsync: An item with the same key has already been added.");

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

            group = _mapper.Map<Group>(command);
            await _groupDataProvider.UpdateAsync(group, cancellationToken).ConfigureAwait(false);

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
                groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, null, cancellationToken, request.GroupType).ConfigureAwait(false)).Select(x => x.GroupId).ToList();
                if (!groupIds.Any())
                {
                    return new GroupDto[] { };
                }
            }
            var groups = await _groupDataProvider.GetByCustomPropertiesAsync(groupIds, request.CustomProperties, null, cancellationToken).ConfigureAwait(false);
            var groupUsers = await _groupUserDataProvider.GetGroupMemberCountByGroupIdsAsync(groupIds, cancellationToken).ConfigureAwait(false);

            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups);
            foreach (var groupDto in groupDtos)
            {
                groupDto.MemberCount = groupUsers.Where(x => x.GroupId == groupDto.Id).Count();
            }

            return groupDtos;
        }

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