using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Domain;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Event;
using SugarChat.Message.Events.Groups;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using SugarChat.Shared.Dtos;
using SugarChat.Shared.Paging;
using SugarChat.Message.Responses.Groups;
using SugarChat.Message.Requests.Groups;
using SugarChat.Core.IRepositories;
using SugarChat.Core.Services.Messages;
using SugarChat.Message;

namespace SugarChat.Core.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IMapper _mapper;
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IMessageDataProvider _messageDataProvider;
        private readonly ISecurityManager _securityManager;

        public GroupService(IMapper mapper, IGroupDataProvider groupDataProvider, IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider, IMessageDataProvider messageDataProvider, ISecurityManager securityManager)
        {
            _mapper = mapper;
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
            _messageDataProvider = messageDataProvider;
            _securityManager = securityManager;
        }

        public async Task<GroupAddedEvent> AddGroupAsync(AddGroupCommand command,
            CancellationToken cancellation = default)
        {
            if (!await _securityManager.IsSupperAdmin())
            {
                User user = await _userDataProvider.GetByIdAsync(command.UserId);
                user.CheckExist(command.UserId);
            }
            Group group = await _groupDataProvider.GetByIdAsync(command.Id, cancellation).ConfigureAwait(false);
            group.CheckNotExist();

            GroupUser groupUser = new()
            {
                UserId = command.UserId,
                GroupId = command.Id,
                Role = UserRole.Owner
            };
            await _groupUserDataProvider.AddAsync(groupUser);
            group = _mapper.Map<Group>(command);
            await _groupDataProvider.AddAsync(group, cancellation).ConfigureAwait(false);

            return _mapper.Map<GroupAddedEvent>(command);
        }

        public async Task<GetGroupsOfUserResponse> GetGroupsOfUserAsync(GetGroupsOfUserRequest request,
            CancellationToken cancellation = default)
        {
            User user = await GetUserAsync(request.Id, cancellation).ConfigureAwait(false);
            user.CheckExist(request.Id);

            IEnumerable<GroupUser> groupUsers = await _groupUserDataProvider.GetByUserIdAsync(request.Id, cancellation).ConfigureAwait(false);
            PagedResult<Group> groups = await _groupDataProvider.GetByIdsAsync(groupUsers.Select(o => o.GroupId), request.PageSettings, cancellation).ConfigureAwait(false);

            var groupsUnreadCountResult = (await _messageDataProvider.GetUserUnreadMessagesByGroupIdsAsync(request.Id, groups.Result.Select(x => x.Id), cancellation))
                               .GroupBy(x => x.GroupId).Select(x => new { GroupId = x.Key, UnreadCount = x.Count() });

            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups.Result);
            foreach (var group in groupDtos)
            {
                group.UnreadCount = groupsUnreadCountResult.FirstOrDefault(x => x.GroupId == group.Id)?.UnreadCount ?? 0;
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
            group.CheckExist(request.GroupId);

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
            var groupIds = (await _groupUserDataProvider.GetByUserIdAsync(request.UserId, cancellationToken).ConfigureAwait(false)).Select(x => x.GroupId).ToArray();
            var groups = await _groupDataProvider.GetByCustomPropertys(request.CustomPropertys, groupIds);
            var groupUsers = await _groupUserDataProvider.GetGroupMemberCountByGroupIdsAsync(groupIds, cancellationToken);

            var groupDtos = _mapper.Map<IEnumerable<GroupDto>>(groups);
            foreach (var groupDto in groupDtos)
            {
                groupDto.MemberCount = groupUsers.Where(x => x.GroupId == groupDto.Id).Count();
            }

            return groupDtos;
        }
    }
}