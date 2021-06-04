using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Events.GroupUsers
{
    public class MessageRemindTypeSetEvent : EventBase
    {
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public MessageRemindType MessageRemindType { get; set; }
    }
}
