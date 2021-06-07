namespace SugarChat.Message.Events.Messages
{
    public class MessageReadSetByUserBasedOnGroupIdEvent : EventBase
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }}
}