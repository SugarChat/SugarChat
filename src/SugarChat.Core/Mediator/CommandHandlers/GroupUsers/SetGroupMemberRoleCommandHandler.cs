﻿using Mediator.Net.Context;
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
    public class SetGroupMemberRoleCommandHandler : ICommandHandler<SetGroupMemberRoleCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;

        public SetGroupMemberRoleCommandHandler(IGroupUserService service)
        {
            _service = service;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetGroupMemberRoleCommand> context, CancellationToken cancellationToken)
        {
            var groupMemberRoleSetEvent = await _service.SetGroupMemberRoleAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupMemberRoleSetEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
