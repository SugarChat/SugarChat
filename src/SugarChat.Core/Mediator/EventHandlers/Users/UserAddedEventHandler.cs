using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
