using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Conversations
{
    public class RemoveConversationCommand : ICommand
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}
