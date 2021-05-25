using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Commands.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class DeleteConversationCommandHandler : ICommandHandler<DeleteConversationCommand>
    {
        public IConversationService _conversationService;
        public DeleteConversationCommandHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public Task Handle(IReceiveContext<DeleteConversationCommand> context, CancellationToken cancellationToken)
        {
            return _conversationService.DeleteConversationByIdAsync(context.Message, cancellationToken);
        }
    }
}
