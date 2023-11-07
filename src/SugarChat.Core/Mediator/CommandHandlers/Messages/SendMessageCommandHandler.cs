using System.Threading;
using System.Threading.Tasks;
using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Messages;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Messages;

namespace SugarChat.Core.Mediator.CommandHandlers.Messages
{
    public class SendMessageCommandHandler : ICommandHandler<SendMessageCommand, SugarChatResponse>
    {
        private readonly IMessageService _messageService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public SendMessageCommandHandler(IMessageService messageService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _messageService = messageService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SendMessageCommand> context, CancellationToken cancellationToken)
        {
            var messageSavedEvent = await _messageService.SaveMessageAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageSavedEvent, cancellationToken).ConfigureAwait(false);
            _backgroundJobClientProvider.Enqueue(() => _messageService.SaveMessageAsync2(context.Message, cancellationToken));
            return new SugarChatResponse();
        }
    }
}
