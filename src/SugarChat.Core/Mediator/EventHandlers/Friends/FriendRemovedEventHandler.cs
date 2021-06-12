using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Friends
{
    public class FriendRemovedEventHandler : IEventHandler<FriendRemovedEvent>
    {
        public Task Handle(IReceiveContext<FriendRemovedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
