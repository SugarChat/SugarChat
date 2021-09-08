using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Messages
{
    public class TranslateMessageCommandHandler : ICommandHandler<TranslateMessageCommand, SugarChatResponse<MessageTranslateDto>>
    {
        private readonly IMessageService _messageService;

        public TranslateMessageCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<MessageTranslateDto>> Handle(IReceiveContext<TranslateMessageCommand> context, CancellationToken cancellationToken)
        {
            var (messageTranslatedEvent, messageTranslateDto) = await _messageService.TranslateMessageAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageTranslatedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<MessageTranslateDto>() { Data = messageTranslateDto };
        }
    }
}
