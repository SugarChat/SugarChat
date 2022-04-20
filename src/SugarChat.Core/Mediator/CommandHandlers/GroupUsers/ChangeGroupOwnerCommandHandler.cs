using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
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
    public class ChangeGroupOwnerCommandHandler : ICommandHandler<ChangeGroupOwnerCommand, SugarChatResponse>    {
        private readonly IGroupUserService _service;

        public ChangeGroupOwnerCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<ChangeGroupOwnerCommand> context, CancellationToken cancellationToken)
        {
            var groupOwnerChangedEvent = await _service.ChangeGroupOwnerAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupOwnerChangedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}