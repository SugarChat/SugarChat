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
    public class UpdateMessageDataCommandHandler : ICommandHandler<UpdateMessageDataCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;

        public UpdateMessageDataCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<UpdateMessageDataCommand> context, CancellationToken cancellationToken)
        {
            await _messageService.UpdateMessageDataAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
