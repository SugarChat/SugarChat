using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Events.Messages
{
    public class MessageRevokedEvent : EventBase
    {
        public string UserId { get; set; }
        public string MessageId { get; set; }
    }
}