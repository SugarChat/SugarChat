namespace SugarChat.Message.Commands.Messages
{
    public class SendMessageCommand : IdRequiredCommand
    {
        public string GroupId { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public string SentBy { get; set; }
        public object Payload { get; set; }
    }
}
