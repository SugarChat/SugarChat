using Mediator.Net.Context;
using Mediator.Net.Contracts;
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
    public class ChangeGroupOwnerHandler : ICommandHandler<ChangeGroupOwnerCommand>
    {
        private readonly IGroupService _groupService;

        public async Task Handle(IReceiveContext<ChangeGroupOwnerCommand> context, CancellationToken cancellationToken)
        {
            await _groupService.ChangeGroupOwner(context.Message, cancellationToken);
        }
    }
}
