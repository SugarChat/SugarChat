using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Users
{
    public class GetCurrentUserCommand : ICommand
    {
        public string Id { get; set; }

    }
}