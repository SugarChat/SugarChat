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

        Task<AddUserToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command, CancellationToken cancellation = default);
        Task<RemoveUserFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command, CancellationToken cancellation = default);
        Task<GetGroupMembersResponse> GetGroupMemberIdsAsync(GetGroupMembersRequest request, CancellationToken cancellation = default);
        Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request, CancellationToken cancellationToken);
        Task<GroupMemberCustomFieldBeSetEvent> SetGroupMemberCustomFieldAsync(SetGroupMemberCustomFieldCommand command, CancellationToken cancellationToken);
        Task<GroupQuittedEvent> QuitGroup(QuitGroupCommand command, CancellationToken cancellation);
        Task<GroupOwnerChangedEvent> ChangeGroupOwner(ChangeGroupOwnerCommand command, CancellationToken cancellation);
        Task<GroupJoinedEvent> JoinGroup(JoinGroupCommand command, CancellationToken cancellation);
        Task<GroupMemberAddedEvent> AddGroupMember(AddGroupMemberCommand command, CancellationToken cancellationToken);
        Task<GroupMemberDeletedEvent> DeleteGroupMember(DeleteGroupMemberCommand command, CancellationToken cancellationToken);
        Task<MessageRemindTypeSetEvent> SetMessageRemindType(SetMessageRemindTypeCommand command, CancellationToken cancellationToken);
        Task<GroupMemberRoleSetEvent> SetGroupMemberRole(SetGroupMemberRoleCommand command, CancellationToken cancellationToken);
    }

}
