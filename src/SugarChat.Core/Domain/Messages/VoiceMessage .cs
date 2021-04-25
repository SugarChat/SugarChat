using System;
using SugarChat.Core.Common;

namespace SugarChat.Core.Domain.Messages
{
    public class VoiceMessage : BaseMessage
    {
        public TimeSpan Duration { get; private set; }

        public VoiceMessage(Guid id, string content, DateTime publishDateTime, Guid @from, Guid to,
            MessageStatus status,
            int order, Guid parentId,TimeSpan duration) : base(id, content, publishDateTime, @from, to, status, order, parentId)
        {
            Duration = CheckDuration(duration);
        }

        private TimeSpan CheckDuration(TimeSpan duration)
        {
            return duration;
        }
    }
}