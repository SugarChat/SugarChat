using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Message.Events.GroupUsers
{
    public class GroupMemberRoleSetEvent : EventBase
    {
        public string GroupId { get; set; }
        public string OwnerId { get; set; }
        public string MemberId { get; set; }
        public UserRole Role { get; set; }
    }
}
