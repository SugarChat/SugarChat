using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Commands.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Messages
{
    public class RevokeMessageCommandHandler : ICommandHandler<RevokeMessageCommand, SugarChatResponse<object>>
    {
        private readonly IMessageService _messageService;
        public RevokeMessageCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse<object>> Handle(IReceiveContext<RevokeMessageCommand> context, CancellationToken cancellationToken)
        {
            var messageRevokedEvent = await _messageService.RevokeMessage(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageRevokedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse<object>();
        }
    }
}
