using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.GroupUsers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.GroupUsers
{
    public class GroupMemberCustomFieldBeSetEventHandler : IEventHandler<GroupMemberCustomFieldBeSetEvent>
    {
        public Task Handle(IReceiveContext<GroupMemberCustomFieldBeSetEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

