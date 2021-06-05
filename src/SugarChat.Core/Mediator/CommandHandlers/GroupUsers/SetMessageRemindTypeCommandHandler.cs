using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class SetMessageRemindTypeCommandHandler : ICommandHandler<SetMessageRemindTypeCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;

        public SetMessageRemindTypeCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetMessageRemindTypeCommand> context, CancellationToken cancellationToken)
        {
            var messageRemindTypeSetEvent = await _service.SetMessageRemindType(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageRemindTypeSetEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
