using Mediator.Net.Context;
using Mediator.Net.Contracts;
using SugarChat.Core.Services.Conversations;
using SugarChat.Message.Commands.Conversations;
using System.Threading;
using System.Threading.Tasks;

namespace SugarChat.Core.Mediator.CommandHandlers.Conversations
{
    public class SetMessageReadCommandHandler : ICommandHandler<SetMessageReadCommand>
    {
        public IConversationService _conversationService;
        public SetMessageReadCommandHandler(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        public Task Handle(IReceiveContext<SetMessageReadCommand> context, CancellationToken cancellationToken)
        {
            return _conversationService.SetMessageReadByConversationIdAsync(context.Message, cancellationToken);

        }
    }
}
