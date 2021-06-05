using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.GroupUsers
{
    public class GroupQuittedEventHandler : IEventHandler<GroupQuittedEvent>
    {
        public Task Handle(IReceiveContext<GroupQuittedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}