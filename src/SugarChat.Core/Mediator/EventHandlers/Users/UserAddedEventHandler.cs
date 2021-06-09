using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Users;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Users
{
    public class UserAddedEventHandler : IEventHandler<UserAddedEvent>
    {
        public Task Handle(IReceiveContext<UserAddedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}


