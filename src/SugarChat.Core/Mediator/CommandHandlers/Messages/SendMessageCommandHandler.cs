using Mediator.Net.Context;
using Mediator.Net.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Core.Services;
using SugarChat.Message.Commands;
using SugarChat.Message.Messages.Events;
using SugarChat.Core.Basic;

namespace SugarChat.Core.Mediator.CommandHandler.Messages
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, SugarChatResponse>
    {
        private readonly ISendMessageService _sendMessageService;
        public SendMessageCommandHandler(ISendMessageService sendMessageService)
        {
            _sendMessageService = sendMessageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            var messageSentEvent = await _sendMessageService.SendMessage(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageSentEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
