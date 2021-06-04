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
        Task<AddUserToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command,
            CancellationToken cancellation = default);

        Task<RemoveUserFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command,
            CancellationToken cancellation = default);

        Task<GetGroupMembersResponse> GetGroupMemberIdsAsync(GetGroupMembersRequest request,
            CancellationToken cancellation = default);

        Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request,
            CancellationToken cancellationToken = default);

        Task<GroupMemberCustomFieldBeSetEvent> SetGroupMemberCustomFieldAsync(SetGroupMemberCustomFieldCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupQuittedEvent> QuitGroup(QuitGroupCommand command, CancellationToken cancellation = default);

        Task<GroupOwnerChangedEvent> ChangeGroupOwner(ChangeGroupOwnerCommand command,
            CancellationToken cancellation = default);

        Task<GroupJoinedEvent> JoinGroup(JoinGroupCommand command, CancellationToken cancellation = default);

        Task<GroupMemberAddedEvent> AddGroupMember(AddGroupMemberCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupMemberDeletedEvent> DeleteGroupMember(DeleteGroupMemberCommand command,
            CancellationToken cancellationToken = default);

        Task<MessageRemindTypeSetEvent> SetMessageRemindType(SetMessageRemindTypeCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupMemberRoleSetEvent> SetGroupMemberRole(SetGroupMemberRoleCommand command,
            CancellationToken cancellationToken = default);
    }
}