using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Users
{
    public class RemoveFriendCommand : ICommand
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }

    }
}