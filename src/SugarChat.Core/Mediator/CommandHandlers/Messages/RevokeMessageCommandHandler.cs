﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SugarChat.Message.Commands.Messages;
using SugarChat.Message.Basic;

namespace SugarChat.Core.Mediator.CommandHandlers.Messages
{
    public class RevokeMessageCommandHandler : ICommandHandler<RevokeMessageCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;
        public RevokeMessageCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RevokeMessageCommand> context, CancellationToken cancellationToken)
        {
            var messageRevokedEvent = await _messageService.RevokeMessageAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageRevokedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
