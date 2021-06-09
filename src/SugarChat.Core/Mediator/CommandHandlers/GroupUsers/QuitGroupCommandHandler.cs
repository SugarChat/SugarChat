﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Basic;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Commands.GroupUsers;
using SugarChat.Message.Events.GroupUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class QuitGroupCommandHandler : ICommandHandler<QuitGroupCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _groupUserService;

        public QuitGroupCommandHandler(IGroupUserService groupUserService)
        {
            _groupUserService = groupUserService;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<QuitGroupCommand> context, CancellationToken cancellationToken)
        {
            var groupQuittedEvent = await _groupUserService.QuitGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupQuittedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}