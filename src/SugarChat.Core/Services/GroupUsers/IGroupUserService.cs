using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Responses;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserService : IService
    {
        Task<AddUserToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command, CancellationToken cancellation);
        Task<RemoveUserFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command, CancellationToken cancellation);
        Task<GetGroupMembersResponse> GetGroupMemberIdsAsync(GetGroupMembersRequest request, CancellationToken cancellation);

    }

}