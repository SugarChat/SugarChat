using System;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Friends
{
    public class AddFriendCommand : ICommand
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public DateTimeOffset BecomeFriendAt { get; set; }
    }
}