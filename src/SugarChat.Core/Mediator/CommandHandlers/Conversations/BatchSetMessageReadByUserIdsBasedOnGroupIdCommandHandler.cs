using Mediator.Net.Context;
using Mediator.Net.Contracts;
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

        public BatchSetMessageReadByUserIdsBasedOnGroupIdCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<BatchSetMessageReadByUserIdsBasedOnGroupIdCommand> context, CancellationToken cancellationToken)
        {
            await _messageService.BatchSetMessageReadByUserIdsBasedOnGroupIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
