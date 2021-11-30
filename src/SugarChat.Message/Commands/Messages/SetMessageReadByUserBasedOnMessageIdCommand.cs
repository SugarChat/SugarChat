using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Messages
{
    public class SetMessageReadByUserBasedOnMessageIdCommand : ICommand, INeedUserExist
    {
        public string UserId { get; set; }
        public string MessageId { get; set; }
    }
}