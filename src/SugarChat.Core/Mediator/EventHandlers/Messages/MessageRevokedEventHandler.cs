using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Messages
{
    public class MessageRevokedEventHandler : IEventHandler<MessageRevokedEvent>
    {
        public Task Handle(IReceiveContext<MessageRevokedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
