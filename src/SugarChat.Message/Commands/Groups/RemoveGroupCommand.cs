using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Groups
{
    public class RemoveGroupCommand : ICommand
    {
        public string Id { get; set; }
    }
}