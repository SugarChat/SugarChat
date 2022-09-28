using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class MigrateGroupUserCustomPropertyCommandHandler : ICommandHandler<MigrateGroupUserCustomPropertyCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _groupUserService;
        public MigrateGroupUserCustomPropertyCommandHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<MigrateGroupUserCustomPropertyCommand> context, CancellationToken cancellationToken)
        {
            await _groupUserService.MigrateCustomProperty(cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
