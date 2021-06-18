using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Groups;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Groups
{
    public class GroupRemovedEventHandler : IEventHandler<GroupRemovedEvent>
    {
        public Task Handle(IReceiveContext<GroupRemovedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
