namespace SugarChat.Message.Events.Users
{
    public class FriendRemovedEvent : EventBase
    {
        public string UserId { get; set; }
        public string FriendId { get; set; }

    }
}