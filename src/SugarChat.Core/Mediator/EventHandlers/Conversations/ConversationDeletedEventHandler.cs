﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Conversations
{
    public class ConversationDeletedEventHandler : IEventHandler<ConversationDeletedEvent>
    {
        public Task Handle(IReceiveContext<ConversationDeletedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
