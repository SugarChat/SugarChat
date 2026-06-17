using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
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
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public BatchAddGroupMemberCommandHandler(IGroupUserService service, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _service = service;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchAddGroupMemberCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _service.BatchAddGroupMembersAsync2(context.Message, cancellationToken));
            await _service.BatchAddGroupMembersAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
