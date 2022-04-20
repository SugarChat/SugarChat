using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class SetMessageReadByUserBasedOnMessageIdCommandHandler : ICommandHandler<SetMessageReadByUserBasedOnMessageIdCommand, SugarChatResponse>
    {
        private IMessageService _messageService;
        public SetMessageReadByUserBasedOnMessageIdCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetMessageReadByUserBasedOnMessageIdCommand> context, CancellationToken cancellationToken)
        {
            var messageReadEvent = await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageReadEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
