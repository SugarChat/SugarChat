using System;

namespace SugarChat.Shared.Dtos
{
    public class MessageDto
    {
        public string GroupId { get; set; }
        public string Content { get; set; }
        public string ParsedContent { get; set; }
        public string Type { get; set; }
        public int SubType { get; set; }
        public string SentBy { get; set; }
        public DateTimeOffset SentTime { get; set; }       
        public bool IsSystem { get; set; }
        public object Payload { get; set; }
    }
}