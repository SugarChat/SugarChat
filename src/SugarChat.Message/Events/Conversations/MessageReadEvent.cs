namespace SugarChat.Message.Events.Conversations
{
    public class MessageReadEvent : EventBase
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}
