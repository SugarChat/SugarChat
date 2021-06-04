using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserService : IService
    {
        Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request, CancellationToken cancellationToken);
        Task<GroupMemberCustomFieldBeSetEvent> SetGroupMemberCustomFieldAsync(SetGroupMemberCustomFieldCommand command, CancellationToken cancellationToken);

    }
}
