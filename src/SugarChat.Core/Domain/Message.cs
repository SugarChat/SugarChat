﻿using SugarChat.Message;
using System;

namespace SugarChat.Core.Domain
{
    public class Message : Entity
    {
        public string GroupId { get; set; }
        public string Content { get; set; }
        public string ParsedContent { get; set; }
        public MessageType Type { get; set; }
        public int SubType { get; set; }
        public string SentBy { get; set; }
        public DateTimeOffset SentTime { get; set; }
        public bool IsSystem { get; set; }
        public object Payload { get; set; }
        public bool IsRevoked { get; set; }
    }
}
