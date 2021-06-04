namespace SugarChat.Message.Events.Conversations
{
    public class ConversationDeletedEvent : EventBase
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}
