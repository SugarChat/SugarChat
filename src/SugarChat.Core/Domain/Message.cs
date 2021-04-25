using System;
using System.Collections.Generic;
using SugarChat.Core.Common;
using SugarChat.Core.Exceptions;

namespace SugarChat.Core.Domain
{
    public class Message : Entity
    {
        public string Content { get; private set; }
        public DateTime PublishDateTime { get; private set; }
        public Guid From { get; private set; }
        public Guid To { get; private set; }
        public MessageStatus Status { get; private set; }
        public Guid ParentId { get; private set; }
        public IEnumerable<Message> Children { get; private set; }
        public int Order { get; private set; }
        public MessageType Type { get; }

        public Message(Guid id,
            string content,
            DateTime publishDateTime,
            Guid from,
            Guid to,
            MessageStatus status,
            int order,
            Guid parentId
        )
        {
            Id = CheckId(id);
            Content = CheckContent(content);
            PublishDateTime = CheckPublishDateTime(publishDateTime);
            From = CheckFrom(from);
            To = CheckTo(to);
            Status = CheckStatus(status);
            Order = CheckOrder(order);
            ParentId = CheckParentId(parentId);
            Type = (MessageType) Enum.Parse(typeof(MessageType), GetType().Name.Replace("Message", ""));
        }

        private int CheckOrder(int order)
        {
            if (order < 1)
            {
                throw new BusinessException("OderShouldGreaterThanZero");
            }

            return order;
        }

        private Guid CheckParentId(Guid parentId)
        {
            return parentId;
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