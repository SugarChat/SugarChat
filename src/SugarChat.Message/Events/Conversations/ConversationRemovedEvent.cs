namespace SugarChat.Message.Events.Conversations
{
    public class ConversationRemovedEvent : EventBase
    {
        public string ConversationId { get; set; }
        public string UserId { get; set; }
    }
}
