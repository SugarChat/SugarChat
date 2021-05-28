using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Groups
{
    public class GroupDismissedEventHandler : IEventHandler<GroupDismissedEvent>
    {
        public Task Handle(IReceiveContext<GroupDismissedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
