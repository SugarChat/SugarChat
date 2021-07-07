using System;

namespace SugarChat.Message.Dtos
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public string SentBy { get; set; }
        public DateTimeOffset SentTime { get; set; }
        public bool IsSystem { get; set; }
        public object Payload { get; set; }
    }
}