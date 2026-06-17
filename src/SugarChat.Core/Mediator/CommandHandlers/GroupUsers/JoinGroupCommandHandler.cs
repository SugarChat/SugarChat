using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.GroupUsers;
using SugarChat.Message.Basic;
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
    public class JoinGroupCommandHandler : ICommandHandler<JoinGroupCommand, SugarChatResponse>    {
        private readonly IGroupUserService _service;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public JoinGroupCommandHandler(IGroupUserService service, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _service = service;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<JoinGroupCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _service.JoinGroupAsync2(context.Message, cancellationToken));
            var groupJoinedEvent = await _service.JoinGroupAsync(context.Message, cancellationToken).ConfigureAwait(false); 
            await context.PublishAsync(groupJoinedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
