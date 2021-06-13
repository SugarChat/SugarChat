using System;
using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Friends
{
    public class AddFriendCommand : IdRequiredCommand
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }
        public DateTimeOffset BecomeFriendAt { get; set; }
    }
}