using SugarChat.Core.Exceptions;
using SugarChat.Core.Services.Groups;
using SugarChat.Core.Services.Users;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUsers
{
    public class GroupUserService : IGroupUserService
    {
        private readonly IUserDataProvider _userDataProvider;
        private readonly IGroupDataProvider _groupDataProvider;
        private readonly IGroupUserDataProvider _groupUserDataProvider;

        public GroupUserService(IGroupDataProvider groupDataProvider,
            IUserDataProvider userDataProvider,
            IGroupUserDataProvider groupUserDataProvider)
        {
            _groupDataProvider = groupDataProvider;
            _userDataProvider = userDataProvider;
            _groupUserDataProvider = groupUserDataProvider;
        }

        public async Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(request.UserId, cancellationToken);
            user.CheckExist(request.UserId);

            var group = await _groupDataProvider.GetByIdAsync(request.GroupId, cancellationToken);
            group.CheckExist(request.GroupId);

            var groupMembers = await _groupUserDataProvider.GetGroupMembersByIdAsync(request.GroupId, cancellationToken);

            return new GetMembersOfGroupResponse
            {
                Result = groupMembers
            };
        }

        public async Task SetGroupMemberCustomFieldAsync(SetGroupMemberCustomFieldCommand command, CancellationToken cancellationToken)
        {
            var user = await _userDataProvider.GetByIdAsync(command.UserId, cancellationToken);
            user.CheckExist(command.UserId);

            var group = await _groupDataProvider.GetByIdAsync(command.GroupId, cancellationToken);
            group.CheckExist(command.GroupId);

            var groupUser = await _groupUserDataProvider.GetByUserAndGroupIdAsync(command.UserId, command.GroupId, cancellationToken);
            groupUser.CheckExist(command.UserId, command.GroupId);

            if (command.CustomProperties != null && command.CustomProperties.Count > 0)
            {
                groupUser.CustomProperties = command.CustomProperties;
                await _groupUserDataProvider.UpdateAsync(groupUser, cancellationToken);
            }
            else
            {
                throw new BusinessWarningException("Custom properties cannot be empty");
            }
        }       
    }
}
