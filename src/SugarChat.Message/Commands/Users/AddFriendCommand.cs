using System.Collections.Generic;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Users
{
    public class AddFriendCommand : ICommand
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }

    }
}