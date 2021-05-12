namespace SugarChat.Message.Events.Users
{
    public class UserDeletedEvent : EventBase
    {
        public string Id { get; set; }
    }
}