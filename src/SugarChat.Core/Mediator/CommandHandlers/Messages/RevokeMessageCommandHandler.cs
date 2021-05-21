using Mediator.Net.Context;
using Mediator.Net.Contracts;
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
    public class RevokeMessageCommandHandler : ICommandHandler<RevokeMessageCommand>
    {
        private readonly IRevokeMessageService _revokeMessageService;
        public RevokeMessageCommandHandler(IRevokeMessageService revokeMessageService)
        {
            _revokeMessageService = revokeMessageService;
        }

        public Task Handle(IReceiveContext<RevokeMessageCommand> context, CancellationToken cancellationToken)
        {
            return _revokeMessageService.RevokeMessage(context.Message, cancellationToken);
        }
    }
}
