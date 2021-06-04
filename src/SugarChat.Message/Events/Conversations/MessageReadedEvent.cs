namespace SugarChat.Message.Events.Conversations
{
    public class MessageReadedEvent : EventBase
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}
