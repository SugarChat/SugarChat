using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands
{
    public class SendMessageCommand : ICommand
    {
        public string Content { get; set; }
    }
}
