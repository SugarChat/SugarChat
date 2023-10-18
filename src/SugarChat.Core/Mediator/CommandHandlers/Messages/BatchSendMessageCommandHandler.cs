using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Messages
{
    public class BatchSendMessageCommandHandler : ICommandHandler<BatchSendMessageCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;

        public BatchSendMessageCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchSendMessageCommand> context, CancellationToken cancellationToken)
        {
            await _messageService.BatchSaveMessageAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
