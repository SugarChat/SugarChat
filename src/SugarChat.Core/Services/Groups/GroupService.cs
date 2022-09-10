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
            using (var transaction = await _transactionManagement.BeginTransactionAsync(cancellation).ConfigureAwait(false))
            {
                try
                {
                    await _groupDataProvider.AddAsync(group, cancellation).ConfigureAwait(false);
                    if (command.CustomProperties != null)
                    {
                        var groupCustomPropertys = new List<Domain.GroupCustomProperty>();
                        foreach (var customProperty in command.CustomProperties)
                        {
                            groupCustomPropertys.Add(new Domain.GroupCustomProperty
                            {
                                GroupId = group.Id,
                                Key = customProperty.Key,
                                Value = customProperty.Value
                            });
                        }
                        await _groupCustomPropertyDataProvider.AddRangeAsync(groupCustomPropertys, cancellation).ConfigureAwait(false);
                    }
                }
                catch (MongoDB.Driver.MongoWriteException ex)
                {
                    if (ex.WriteError.Code == 11000)
                    {
                        group.CheckNotExist();
                    }
                    await transaction.RollbackAsync(cancellation).ConfigureAwait(false);
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
            User user = await GetUserAsync(request.UserId, cancellation).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellation, request.Type).ConfigureAwait(false);
            PagedResult<Group> groups = await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), request.PageSettings, cancellation).ConfigureAwait(false);
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupIds(groups.Result.Select(x => x.Id)).ConfigureAwait(false);
            foreach (var group in groups.Result)
            {
                var _groupCustomProperties = groupCustomProperties.Where(x => x.GroupId == group.Id).ToList();
                group.CustomProperties = _groupCustomProperties;
            }
            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups.Result);
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

        private async Task<User> GetUserAsync(string id, CancellationToken cancellation = default)
        {
            return await _userDataProvider.GetByIdAsync(id, cancellation).ConfigureAwait(false);
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
            var groupCustomProperties = await _groupCustomPropertyDataProvider.GetPropertiesByGroupId(group.Id);
            group.CustomProperties = groupCustomProperties;
            var groupDto = _mapper.Map<GroupDto>(group);
            groupDto.MemberCount =
                await _groupUserDataProvider.GetGroupMemberCountByGroupIdAsync(request.GroupId, cancellationToken).ConfigureAwait(false);

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
            groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken, request.Type).ConfigureAwait(false)).Select(x => x.GroupId).ToList();
            if (!groupIds.Any())
            {
                return new GroupDto[] { };
            }
            var groups = await _groupDataProvider.GetByCustomProperties(groupIds, request.CustomProperties, null, cancellationToken).ConfigureAwait(false);
            var groupUsers = await _groupUserDataProvider.GetGroupMemberCountByGroupIdsAsync(groupIds, cancellationToken).ConfigureAwait(false);

            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups);
            foreach (var groupDto in groupDtos)
            {
                groupDto.MemberCount = groupUsers.Where(x => x.GroupId == groupDto.Id).Count();
            }

            return groupDtos;
        }
    }
}