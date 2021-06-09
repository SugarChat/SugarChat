using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Push;
using SugarChat.Message.Events.Messages;

namespace SugarChat.Core.Mediator.EventHandlers.Messages
{
    public class MessageSavedEventHandler : IEventHandler<MessageSavedEvent>
    {
        private readonly List<IPushService> _pushServices;

        public MessageSavedEventHandler(List<IPushService> pushServices)
        {
            _pushServices = pushServices;
        }

        public async Task Handle(IReceiveContext<MessageSavedEvent> context, CancellationToken cancellationToken)
        {
            foreach (var pushService in _pushServices)
            {
                await pushService.Push(context.Message, cancellationToken);
            }
        }
    }
}
