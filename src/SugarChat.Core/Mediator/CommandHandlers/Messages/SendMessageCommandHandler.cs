using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Message.Command;
using System;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Services;
using SugarChat.Message.Messages.Events;

namespace SugarChat.Core.Mediator.CommandHandler.Messages
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand>
    {
        private readonly ISendMessageService _sendMessageService;
        public SendMessageCommandHandler(ISendMessageService sendMessageService)
        {
            _sendMessageService = sendMessageService;
        }

        public async Task Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            var messageSentEvent = await _sendMessageService.SendMessage(context.Message, cancellationToken).ConfigureAwait(false);
            //await context.PublishAsync(messageSentEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
