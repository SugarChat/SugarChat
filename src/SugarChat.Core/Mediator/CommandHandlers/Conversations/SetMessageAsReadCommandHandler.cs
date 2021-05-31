using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Commands.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class SetMessageAsReadCommandHandler : ICommandHandler<SetMessageAsReadCommand>
    {
        public IConversationService _conversationService;
        public SetMessageAsReadCommandHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public Task Handle(IReceiveContext<SetMessageAsReadCommand> context, CancellationToken cancellationToken)
        {
            return _conversationService.SetMessageAsReadByConversationIdAsync(context.Message, cancellationToken);

        }
    }
}
