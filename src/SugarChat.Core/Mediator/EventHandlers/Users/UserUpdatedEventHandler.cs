using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Users;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Users
{
    public class UserUpdatedEventHandler : IEventHandler<UserUpdatedEvent>
    {
        public Task Handle(IReceiveContext<UserUpdatedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}


