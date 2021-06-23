using SugarChat.Message;

namespace SugarChat.SignalR.Client.ConsoleSample
{
    public class SendMessageCommand
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string Content { get; set; }
        public MessageType Type { get; set; }
        public string SentBy { get; set; }
        public object Payload { get; set; }
    }
}
