using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Conversations
{
    public class MessageReadedEventHandler : IEventHandler<MessageReadEvent>
    {
        public Task Handle(IReceiveContext<MessageReadEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
