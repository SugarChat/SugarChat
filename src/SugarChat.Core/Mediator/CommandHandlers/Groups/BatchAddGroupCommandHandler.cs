using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
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
    public class BatchAddGroupCommandHandler : ICommandHandler<BatchAddGroupCommand, SugarChatResponse>
    {
        private readonly IGroupService _groupService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public BatchAddGroupCommandHandler(IGroupService groupService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _groupService = groupService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchAddGroupCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _groupService.BatchAddGroupAsync2(context.Message, cancellationToken));
            await _groupService.BatchAddGroupAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
