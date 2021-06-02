using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Users
{
    public class RemoveUserCommand : ICommand
    {
        public string Id { get; set; }
    }
}