using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Basic;
using SugarChat.Message.Commands.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class DeleteConversationCommandHandler : ICommandHandler<RemoveConversationCommand,SugarChatResponse>
    {
        public IConversationService _conversationService;
        private readonly IBackgroundJobClientProvider _backgroundJobClientProvider;

        public DeleteConversationCommandHandler(IConversationService conversationService, IBackgroundJobClientProvider backgroundJobClientProvider)
        {
            _conversationService = conversationService;
            _backgroundJobClientProvider = backgroundJobClientProvider;
        }

        public async Task<SugarChatResponse> Handle(IReceiveContext<RemoveConversationCommand> context, CancellationToken cancellationToken)
        {
            var conversationDeletedEvent = await _conversationService.RemoveConversationByConversationIdAsync(context.Message, cancellationToken).ConfigureAwait(false);
            _backgroundJobClientProvider.Enqueue(() => _conversationService.RemoveConversationByConversationIdAsync2(context.Message, cancellationToken));
            await context.PublishAsync(conversationDeletedEvent, cancellationToken).ConfigureAwait(false);
            return new SugarChatResponse();
        }
    }
}
