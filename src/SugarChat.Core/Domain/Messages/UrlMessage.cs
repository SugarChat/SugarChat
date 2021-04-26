using System;
using SugarChat.Core.Common;

namespace SugarChat.Core.Domain.Messages
{
    public class UrlMessage : BaseMessage
    {
        public Uri Uri { get; private set; }
        public UrlMessage(Guid id, string content, DateTime publishDateTime, Guid @from, Guid to, MessageStatus status,
            int order, Guid parentId, Uri uri) : base(id, content, publishDateTime, @from, to, status, order, parentId)
        {
            Uri = CheckUri(uri);

        }

        private Uri CheckUri(Uri uri)
        {
            return uri;
        }
    }
}