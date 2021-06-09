using Mediator.Net.Contracts;

namespace SugarChat.Message.Events.Messages
{
    public class MessageSavedEvent : IEvent
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public string SentBy { get; set; }
        public object Payload { get; set; }
    }
}
