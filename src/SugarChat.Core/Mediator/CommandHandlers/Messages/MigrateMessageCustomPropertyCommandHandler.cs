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
    public class MigrateMessageCustomPropertyCommandHandler : ICommandHandler<MigrateMessageCustomPropertyCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;
        public MigrateMessageCustomPropertyCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<MigrateMessageCustomPropertyCommand> context, CancellationToken cancellationToken)
        {
            await _messageService.MigrateCustomPropertyAsync(cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
