using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Groups;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Groups
{
    public class MigrateGroupCustomPropertyCommandHandler : ICommandHandler<MigrateGroupCustomPropertyCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;
        public MigrateGroupCustomPropertyCommandHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<MigrateGroupCustomPropertyCommand> context, CancellationToken cancellationToken)
        {
            await _groupService.MigrateCustomProperty(cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
