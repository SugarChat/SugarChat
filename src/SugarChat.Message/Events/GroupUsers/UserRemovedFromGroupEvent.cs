namespace SugarChat.Message.Events.GroupUsers
{
    public class UserRemovedFromGroupEvent : EventBase
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}