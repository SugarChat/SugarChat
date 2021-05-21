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
    public class DismissGroupCommandHandler : ICommandHandler<DismissGroupCommand>
    {
        private readonly IGroupService _groupService;

        public DismissGroupCommandHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public Task Handle(IReceiveContext<DismissGroupCommand> context, CancellationToken cancellationToken)
        {
            return _groupService.DismissGroup(context.Message, cancellationToken);
        }
    }
}
