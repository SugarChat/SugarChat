using System;
using SugarChat.Core.Common;

namespace SugarChat.Core.Domain.Messages
{
    public class ImageMessage : Message
    {
        public ImageFormat Format { get; private set; }
        public ImageMessage(Guid id, string content, DateTime publishDateTime, Guid @from, Guid to,
            MessageStatus status,
            int order, Guid parentId, ImageFormat format) : base(id, content, publishDateTime, @from, to, status, order, parentId)
        {
            Format = CheckFormat(format);
        }

        private ImageFormat CheckFormat(ImageFormat format)
        {
            return format;
        }
    }
}