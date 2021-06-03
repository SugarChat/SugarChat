using Mediator.Net.Contracts;

namespace SugarChat.Message.Commands.Friends
{
    public class RemoveFriendCommand : ICommand
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }

    }
}