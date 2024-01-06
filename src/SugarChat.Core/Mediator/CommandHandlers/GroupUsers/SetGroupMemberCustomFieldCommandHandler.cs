﻿using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.GroupUsers;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.GroupUsers
{
    public class SetGroupMemberCustomFieldCommandHandler : ICommandHandler<SetGroupMemberCustomFieldCommand, SugarChatResponse>
    {
        public IGroupUserService _groupUserService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public SetGroupMemberCustomFieldCommandHandler(IGroupUserService groupUserService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupUserService = groupUserService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }
        public async Task<SugarChatResponse> Handle(IReceiveContext<SetGroupMemberCustomFieldCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupUserService.SetGroupMemberCustomPropertiesAsync2(context.Message, cancellationToken));
            var groupMemberCustomFieldBeSetEvent = await _groupUserService.SetGroupMemberCustomPropertiesAsync(context.Message, cancellationToken);
            await context.PublishAsync(groupMemberCustomFieldBeSetEvent, cancellationToken);
            return new SugarChatResponse();
        }
    }
}
