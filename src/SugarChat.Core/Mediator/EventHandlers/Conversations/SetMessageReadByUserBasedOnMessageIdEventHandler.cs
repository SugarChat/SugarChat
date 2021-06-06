using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.EventHandlers.Conversations
{
    public class SetMessageReadByUserBasedOnMessageIdEventHandler : IEventHandler<SetMessageReadByUserBasedOnMessageIdEvent>
    {
        public Task Handle(IReceiveContext<SetMessageReadByUserBasedOnMessageIdEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
