using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Groups;
using SugarChat.Message.Commands.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class BatchAddGroupMemberCommandHandler : ICommandHandler<BatchAddGroupMemberCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;

        public BatchAddGroupMemberCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchAddGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            await _service.BatchAddGroupMembersAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
