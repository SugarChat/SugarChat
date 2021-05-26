using System;

namespace SugarChat.Core.Domain
{
    public class GroupUser : Entity
    {
        public string UserId { get; set; }
        public string GroupId { get; set; }
        public bool IsMaster { get; set; }
        public bool IsAdmin { get; set; }
    }
}
