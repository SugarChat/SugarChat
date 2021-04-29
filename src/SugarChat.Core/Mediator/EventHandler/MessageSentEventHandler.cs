using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Push;
using SugarChat.Message.Event;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandler
{
    public class MessageSentEventHandler : IEventHandler<MessageSentEvent>
    {
        private readonly List<IPushService> _pushServices;

        public MessageSentEventHandler(List<IPushService> pushServices)
        {
            _pushServices = pushServices;
        }

        public async Task Handle(IReceiveContext<MessageSentEvent> context, CancellationToken cancellationToken)
        {
            foreach (var pushService in _pushServices)
            {
                await pushService.Push(context.Message, cancellationToken);
            }
        }
    }
}
