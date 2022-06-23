using SugarChat.Message;
using System;
using System.Collections.Generic;

namespace SugarChat.Core.Domain
{
    public class Message : Entity
    {
        public string GroupId { get; set; }
        public string Content { get; set; }
        public int Type { get; set; }
        public string SentBy { get; set; }
        public DateTimeOffset SentTime { get; set; }
        public bool IsSystem { get; set; }
        public string Payload { get; set; }
        public bool IsRevoked { get; set; }
        public IEnumerable<MessageCustomProperty> CustomProperties { get; set; }
    }
}
