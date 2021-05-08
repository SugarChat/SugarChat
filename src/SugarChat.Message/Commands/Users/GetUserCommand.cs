using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Users
{
    public class GetUserCommand : ICommand
    {
        public string Id { get; set; }

    }
}