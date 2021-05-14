using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Command;
using System;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Services;

namespace SugarChat.Core.Mediator.CommandHandler
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly ISendMessageService _sendMessageService;
        public SendMessageCommandHandler(ISendMessageService sendMessageService)
        {
            _sendMessageService = sendMessageService;
        }

        public Task Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            return _sendMessageService.SendMessage(context.Message, cancellationToken);
        }
    }
}
