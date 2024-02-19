﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
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
    public class DeleteGroupMemberCommandHandler : ICommandHandler<RemoveGroupMemberCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public DeleteGroupMemberCommandHandler(IGroupUserService service, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _service = service;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _service.RemoveGroupMembersAsync2(context.Message, cancellationToken));
            var groupMemberDeletedEvent = await _service.RemoveGroupMembersAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(groupMemberDeletedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}