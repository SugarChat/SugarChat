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
using SugarChat.Core.Exceptions;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IMessageDataProvider _messageDataProvider;

        public GroupService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider, IMessageDataProvider messageDataProvider)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
        }

        public async Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command,
            CancellationToken cancellation = default)
        {
            User user = await _userDataProvider.GetByIdAsync(command.UserId, cancellation);
            user.CheckExist(command.UserId);
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation).ConfigureAwait(false);
            group.CheckNotExist();

            GroupUser groupUser = new()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = command.UserId,
                GroupId = command.Id,
                Role = UserRole.Owner,
                CreatedBy = command.CreatedBy
            };
            await _groupUserDataProvider.AddAsync(groupUser, cancellation);
            group = _mapper.Map<Group>(command);
            try
            {
                await _groupDataProvider.AddAsync(group, cancellation).ConfigureAwait(false);
            }
            catch (MongoDB.Driver.MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000)
                {
                    group.CheckNotExist();
                }
                throw new BusinessWarningException(Prompt.DatabaseError, ex.InnerException);
            }
            catch (Exception)
            {
                throw;
            }
            return _mapper.Map<GroupAddedEvent>(command);
        }

        public async Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(request.UserId, cancellation).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellation).ConfigureAwait(false);
            PagedResult<Group> groups = await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), request.PageSettings, cancellation).ConfigureAwait(false);

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
            await _groupDataProvider.RemoveAsync(group, cancellation).ConfigureAwait(false);
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

            var groupUser =
                await _groupUserDataProvider.GetByUserAndGroupIdAsync(request.UserId, request.GroupId,
                    cancellationToken).ConfigureAwait(false);
            groupUser.CheckExist(request.UserId, request.GroupId);

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
            await _messageDataProvider.RemoveRangeAsync(messages, cancellation).ConfigureAwait(false);

            var groupUsers = await _groupUserDataProvider.GetByGroupIdAsync(command.GroupId, cancellation).ConfigureAwait(false);
            await _groupUserDataProvider.RemoveRangeAsync(groupUsers, cancellation).ConfigureAwait(false);

            await _groupDataProvider.RemoveAsync(group, cancellation).ConfigureAwait(false);

            return _mapper.Map<GroupDismissedEvent>(command);
        }

        public async Task<IEnumerable<GroupDto>> GetByCustomProperties(GetGroupByCustomPropertiesRequest request, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken).ConfigureAwait(false);
            user.CheckExist(request.UserId);

            List<string> groupIds = new List<string>();
            if (!request.SearchAllGroup)
            {
                groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken).ConfigureAwait(false)).Select(x => x.GroupId).ToList();
                if (!groupIds.Any())
                {
                    return new GroupDto[] { };
                }
            }
            var groups = await _groupDataProvider.GetByCustomProperties(request.CustomProperties, groupIds, cancellationToken).ConfigureAwait(false);
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