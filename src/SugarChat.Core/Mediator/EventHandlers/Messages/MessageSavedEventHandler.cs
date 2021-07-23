using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Push;
using SugarChat.Message.Events.Messages;
using SugarChat.SignalR.Enums;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.Core.Mediator.EventHandlers.Messages
{
    public class MessageSavedEventHandler : IEventHandler<MessageSavedEvent>
    {
        public Task Handle(IReceiveContext<MessageSavedEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
