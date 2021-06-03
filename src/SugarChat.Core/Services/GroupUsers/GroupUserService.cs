using AutoMapper;
using SugarChat.Core.Exceptions;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {

        private readonly IGroupUserDataProvider _groupUserDataProvider;
        private readonly IMapper _mapper;
        public GroupUserService(IGroupUserDataProvider groupUserDataProvider,
            IMapper mapper)
        {
            _groupUserDataProvider = groupUserDataProvider;
            _mapper = mapper;
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
    }
}
