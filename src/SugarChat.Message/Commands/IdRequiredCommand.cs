using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands
{
    public class IdRequiredCommand : ICommand
    {
        public string Id { get; set; }
    }
}