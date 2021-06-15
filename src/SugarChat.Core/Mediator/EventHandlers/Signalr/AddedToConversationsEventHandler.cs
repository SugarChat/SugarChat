using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Events.SignalR;
using SugarChat.SignalR.ServerClient;

namespace SugarChat.Core.Mediator.EventHandlers.Signalr
{
    public class AddedToConversationsEventHandler : IEventHandler<AddedToConversationsEvent>
    {
        public Task Handle(IReceiveContext<AddedToConversationsEvent> context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
