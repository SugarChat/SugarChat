using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.SignalR.Enums;
using SugarChat.SignalR.Server.Models;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.Core.Mediator.EventHandlers.Messages
{
    public class MessageSavedEventHandler : IEventHandler<MessageSavedEvent>
    {
        private readonly IServerClient _client;

        public MessageSavedEventHandler(IServerClient client)
        {
            _client = client;
        }

        public async Task Handle(IReceiveContext<MessageSavedEvent> context, CancellationToken cancellationToken)
        {
            await _client.SendMessage(new SendMessageModel()
            {
                SendTo = "",
                Messages = new[] {context.Message.SentBy, context.Message.Content},
                SendWay = SendWay.All
            });
        }
    }
}