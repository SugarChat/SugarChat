using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Users
{
    public class AddUserCommand : IdRequiredCommand
    {
        public Dictionary<string, string> CustomProperties { get; set; }
        public string DisplayName { get; set; }
        public string AvatarUrl { get; set; }
    }
}