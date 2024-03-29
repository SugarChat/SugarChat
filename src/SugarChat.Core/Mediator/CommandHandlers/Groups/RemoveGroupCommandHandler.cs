﻿using Mediator.Net.Context;
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
    public class RemoveGroupCommandHandler : ICommandHandler<RemoveGroupCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;

        public RemoveGroupCommandHandler(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveGroupCommand> context, CancellationToken cancellationToken)
        {
            var groupRemovedEvent = await _groupService.RemoveGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
