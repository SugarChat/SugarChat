using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class JoinGroupHandler : ICommandHandler<JoinGroupCommand>
    {
        private readonly IGroupUserService _service;

        public JoinGroupHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task Handle(IReceiveContext<JoinGroupCommand> context, CancellationToken cancellationToken)
        {
            var groupJoinedEvent = await _service.JoinGroup(context.Message, cancellationToken).ConfigureAwait(false); 
            await context.PublishAsync(groupJoinedEvent, cancellationToken).ConfigureAwait(false);
        }
    }
}
