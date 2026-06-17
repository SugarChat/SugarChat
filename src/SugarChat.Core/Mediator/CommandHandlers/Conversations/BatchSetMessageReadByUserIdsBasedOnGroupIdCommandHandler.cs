using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class BatchSetMessageReadByUserIdsBasedOnGroupIdCommandHandler : ICommandHandler<BatchSetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>
    {
        private IMessageService _messageService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public BatchSetMessageReadByUserIdsBasedOnGroupIdCommandHandler(IMessageService messageService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _messageService = messageService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchSetMessageReadByUserIdsBasedOnGroupIdCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _messageService.BatchSetMessageReadByUserIdsBasedOnGroupIdAsync2(context.Message, cancellationToken));
            await _messageService.BatchSetMessageReadByUserIdsBasedOnGroupIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
