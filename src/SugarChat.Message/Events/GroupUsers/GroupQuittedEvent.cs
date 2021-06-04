using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Events.GroupUsers
{
    public class GroupQuittedEvent : EventBase
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
    }
}
