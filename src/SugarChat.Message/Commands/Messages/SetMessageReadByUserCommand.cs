using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Messages
{
    public class SetMessageReadByUserCommand : ICommand
    {
        public string UserId { get; set; }
        public string MessageId { get; set; }
    }
}