using System;
using SugarChat.Core.Common;

namespace SugarChat.Core.Domain.Messages
{
    public class TextMessage : BaseMessage
    {
        public TextMessage(Guid id, string content, DateTime publishDateTime, Guid @from, Guid to, MessageStatus status,
            int order, Guid parentId) : base(id, content, publishDateTime, @from, to, status, order,parentId)
        {
        }
    }
}