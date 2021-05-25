using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Conversations
{
    public class DeleteConversationCommand : ICommand
    {
        public string ConversationId { get; set; }
    }
}
