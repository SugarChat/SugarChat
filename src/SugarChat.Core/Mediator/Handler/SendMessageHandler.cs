using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Command;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.Handler
{
    public class SendMessageHandler : ICommandHandler<SendMessageCommand>
    {
        public Task Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
