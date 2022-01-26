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
    public class RemoveAllGroupMemberCommandHandler : ICommandHandler<RemoveAllGroupMemberCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;

        public RemoveAllGroupMemberCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveAllGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            await _service.RemoveAllGroupMemberAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
