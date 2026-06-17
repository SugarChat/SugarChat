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
    public class SetMessageReadByUserBasedOnMessageIdCommandHandler : ICommandHandler<SetMessageReadByUserBasedOnMessageIdCommand, SugarChatResponse>
    {
        private IMessageService _messageService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public SetMessageReadByUserBasedOnMessageIdCommandHandler(IMessageService messageService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _messageService = messageService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<SetMessageReadByUserBasedOnMessageIdCommand> context, CancellationToken cancellationToken)
        {
            _backgroundJobClientProvider.Enqueue(() => _messageService.SetMessageReadByUserBasedOnMessageIdAsync2(context.Message, cancellationToken));
            var messageReadEvent = await _messageService.SetMessageReadByUserBasedOnMessageIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            await context.PublishAsync(messageReadEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
