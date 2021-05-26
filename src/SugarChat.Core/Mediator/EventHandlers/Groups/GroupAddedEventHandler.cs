using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Groups;

namespace SugarChat.Core.Mediator.EventHandlers.Groups
{
    public class GroupAddedEventHandler : IEventHandler<GroupAddedEvent>
    {
        public Task Handle(IReceiveContext<GroupAddedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}