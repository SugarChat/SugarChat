using System;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using SugarChat.Message.Requests;
using SugarChat.Message.Requests.GroupUsers;
using SugarChat.Message.Responses;
using SugarChat.Message.Responses.GroupUsers;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserService : IService
    {
        Task<UserAddedToGroupEvent> AddUserToGroupAsync(AddUserToGroupCommand command,
            CancellationToken cancellationToken = default);

        Task<UserRemovedFromGroupEvent> RemoveUserFromGroupAsync(RemoveUserFromGroupCommand command,
            CancellationToken cancellationToken = default);

        Task<GetGroupMembersResponse> GetGroupMemberIdsAsync(GetGroupMembersRequest request,
            CancellationToken cancellationToken = default);

        Task<GetMembersOfGroupResponse> GetGroupMembersByIdAsync(GetMembersOfGroupRequest request,
            CancellationToken cancellationToken = default);

        Task<GroupMemberCustomFieldSetEvent> SetGroupMemberCustomPropertiesAsync(SetGroupMemberCustomFieldCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupQuittedEvent> QuitGroupAsync(QuitGroupCommand command, CancellationToken cancellationToken = default);

        Task<GroupOwnerChangedEvent> ChangeGroupOwnerAsync(ChangeGroupOwnerCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupJoinedEvent> JoinGroupAsync(JoinGroupCommand command, CancellationToken cancellationToken = default);

        Task<GroupMemberAddedEvent> AddGroupMembersAsync(AddGroupMemberCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupMemberRemovedEvent> RemoveGroupMembersAsync(RemoveGroupMemberCommand command,
            CancellationToken cancellationToken = default);

        Task<MessageRemindTypeSetEvent> SetMessageRemindTypeAsync(SetMessageRemindTypeCommand command,
            CancellationToken cancellationToken = default);

        Task<GroupMemberRoleSetEvent> SetGroupMemberRoleAsync(SetGroupMemberRoleCommand command,
            CancellationToken cancellationToken = default);

        Task<GetUserIdsByGroupIdsResponse> GetUsersByGroupIdsAsync(GetUserIdsByGroupIdsRequest request, CancellationToken cancellationToken = default);

        Task UpdateGroupUserDataAsync(UpdateGroupUserDataCommand command, CancellationToken cancellationToken = default);

        Task MigrateCustomPropertyAsync(CancellationToken cancellation = default);

        [Obsolete("user CheckUserIdsInGroupAsync")]
        Task<bool> CheckUserIsInGroupAsync(CheckUserIsInGroupCommand command, CancellationToken cancellation = default);

        Task<bool> CheckUserIdsInGroupAsync(CheckUserIdsInGroupCommand command, CancellationToken cancellation = default);

        Task MigrateGroupCustomPropertyAsyncToGroupUser(int pageSize, CancellationToken cancellation = default);
    }
}