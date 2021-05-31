using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Events.GroupUsers
{
    public class GroupOwnerChangedEvent : EventBase
    {

        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string GroupId { get; set; }
    }
}
