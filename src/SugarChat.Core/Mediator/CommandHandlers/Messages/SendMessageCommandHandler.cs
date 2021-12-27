using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;

namespace SugarChat.Core.Mediator.CommandHandlers.Messages
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;
        public SendMessageCommandHandler(IMessageService messageService)
        {
            _messageService = messageService;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            var messageSavedEvent = await _messageService.SaveMessageAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageSavedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
