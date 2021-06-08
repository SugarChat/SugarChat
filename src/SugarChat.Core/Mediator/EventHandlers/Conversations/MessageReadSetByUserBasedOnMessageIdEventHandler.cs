using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Conversations
{
    public class MessageReadSetByUserBasedOnMessageIdEventHandler : IEventHandler<MessageReadSetByUserBasedOnMessageIdEvent>
    {
        public Task Handle(IReceiveContext<MessageReadSetByUserBasedOnMessageIdEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
