using Mediator.Net.Context;
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
    public class SetGroupMemberRoleCommandHandler : ICommandHandler<SetGroupMemberRoleCommand, SugarChatResponse>
    {
        private readonly IGroupUserService _service;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public SetGroupMemberRoleCommandHandler(IGroupUserService service, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _service = service;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetGroupMemberRoleCommand> context, CancellationToken cancellationToken)
        {
            var groupMemberRoleSetEvent = await _service.SetGroupMemberRoleAsync(context.Message, cancellationToken).ConfigureAwait(false);
            _backgroundJobClientProvider.Enqueue(() => _service.SetGroupMemberRoleAsync2(context.Message, cancellationToken));
            await context.PublishAsync(groupMemberRoleSetEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
