using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class SetMessageReadByUserIdsBasedOnGroupIdCommandHandler : ICommandHandler<SetMessageReadByUserIdsBasedOnGroupIdCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;

        public SetMessageReadByUserIdsBasedOnGroupIdCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetMessageReadByUserIdsBasedOnGroupIdCommand> context, CancellationToken cancellationToken)
        {
            var messageReadEvent = await _messageService.SetMessageReadByUserIdsBasedOnGroupIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageReadEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
