using System;
using SugarChat.Core.Common;

namespace SugarChat.Core.Domain
{
    public class Message : Entity
    {
        public string Content { get; private set; }
        public DateTime PublishDateTime { get; private set; }
        public Guid From { get; private set; }
        public Guid To { get; private set; }
        public MessageStatus Status { get; private set; }
    }
}