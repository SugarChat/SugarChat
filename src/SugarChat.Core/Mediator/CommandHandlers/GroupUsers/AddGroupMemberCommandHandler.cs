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

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUser
{
	public class AddGroupMemberCommandHandler : ICommandHandler<AddGroupMemberCommand, SugarChatResponse>    {
        private readonly IGroupUserService _service;

        public AddGroupMemberCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<AddGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            var groupMemberAddedEvent = await _service.AddGroupMembersAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupMemberAddedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
