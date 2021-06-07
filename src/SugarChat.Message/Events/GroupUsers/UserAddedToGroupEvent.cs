namespace SugarChat.Message.Events.GroupUsers
{
    public class UserAddedToGroupEvent : EventBase
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}