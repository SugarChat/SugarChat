using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Commands.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Groups
{
    public class DismissGroupCommandHandler : ICommandHandler<DismissGroupCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;

        public DismissGroupCommandHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<DismissGroupCommand> context, CancellationToken cancellationToken)
        {
            var groupDismissedEvent = await _groupService.DismissGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupDismissedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
