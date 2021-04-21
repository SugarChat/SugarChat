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

        public Message(Guid id, 
            string content,
            DateTime publishDateTime,
            Guid from,
            Guid to,
            MessageStatus status
            )
        {
            Id = CheckId(id);
            Content = CheckContent(content);
            PublishDateTime = CheckPublishDateTime(publishDateTime);
            From = CheckFrom(from);
            To = CheckTo(to);
            Status = CheckStatus(status);
        }

        private MessageStatus CheckStatus(MessageStatus status)
        {
            return status;
        }

        private Guid CheckTo(Guid to)
        {
            return to;
        }

        private Guid CheckFrom(Guid from)
        {
            return from;
        }

        private DateTime CheckPublishDateTime(DateTime publishDateTime)
        {
            return publishDateTime;
        }

        private string CheckContent(string content)
        {
            return content;
        }

        private Guid CheckId(Guid id)
        {
            return id;
        }
    }
}