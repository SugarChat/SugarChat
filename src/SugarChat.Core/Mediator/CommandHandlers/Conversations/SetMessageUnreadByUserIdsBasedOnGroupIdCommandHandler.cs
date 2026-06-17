using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class SetMessageUnreadByUserIdsBasedOnGroupIdCommandHandler : ICommandHandler<SetMessageUnreadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public SetMessageUnreadByUserIdsBasedOnGroupIdCommandHandler(IMessageService messageService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _messageService = messageService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetMessageUnreadByUserIdsBasedOnGroupIdCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _messageService.SetMessageUnreadByUserIdsBasedOnGroupIdAsync2(context.Message, cancellationToken));
            await _messageService.SetMessageUnreadByUserIdsBasedOnGroupIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
