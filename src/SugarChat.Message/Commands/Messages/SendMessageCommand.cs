namespace SugarChat.Message.Commands.Messages
{
    public class SendMessageCommand : IdRequiredCommand
    {
        public string GroupId { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public string SentBy { get; set; }
        public string Payload { get; set; }
    }
}
