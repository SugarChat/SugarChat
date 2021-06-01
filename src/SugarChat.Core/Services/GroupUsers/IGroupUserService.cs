using SugarChat.Core.Domain;
using SugarChat.Core.IRepositories;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Services.GroupUsers
{
    public interface IGroupUserService : IService
    {
        Task<GroupQuittedEvent> QuitGroup(QuitGroupCommand command, CancellationToken cancellation);
        Task<GroupOwnerChangedEvent> ChangeGroupOwner(ChangeGroupOwnerCommand command, CancellationToken cancellation);
        Task<GroupJoinedEvent> JoinGroup(JoinGroupCommand command, CancellationToken cancellation);
        Task<GroupMemberAddedEvent> AddGroupMember(AddGroupMemberCommand command, CancellationToken cancellationToken);
        Task<GroupMemberDeletedEvent> DeleteGroupMember(DeleteGroupMemberCommand command, CancellationToken cancellationToken);
        Task<MessageRemindTypeSetEvent> SetMessageRemindType(SetMessageRemindTypeCommand command, CancellationToken cancellationToken);
        Task<GroupMemberRoleSetEvent> SetGroupMemberRole(SetGroupMemberRoleCommand command, CancellationToken cancellationToken);
    }
}