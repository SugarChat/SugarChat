using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.GroupUsers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.GroupUsers
{
    public class GroupMemberCustomFieldBeSetEventHandler : IEventHandler<GroupMemberCustomFieldSetEvent>
    {
        public Task Handle(IReceiveContext<GroupMemberCustomFieldSetEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}

