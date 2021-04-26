using System;
using SugarChat.Core.Common;

namespace SugarChat.Core.Domain.Messages
{
    public class VideoMessage : BaseMessage
    {
        public VideoFormat Format { get; private set; }
        public VideoEncoder Encoder { get; private set; }

        public VideoMessage(Guid id, string content, DateTime publishDateTime, Guid @from, Guid to,
            MessageStatus status,
            int order, Guid? parentId, VideoFormat format, VideoEncoder encoder) : base(id, content, publishDateTime,
            @from, to, status, order, parentId)
        {
            Format = CheckFormat(format);
            Encoder = CheckEncoder(encoder);
        }

        private VideoEncoder CheckEncoder(VideoEncoder encoder)
        {
            return encoder;
        }


        private VideoFormat CheckFormat(VideoFormat format)
        {
            return format;
        }
    }
}