using System;
using System.Collections.Generic;

namespace SugarChat.Core.Domain
{
    public class Group : Receiver
    {
        public string Name { get; private set; }
        public virtual ICollection<Guid> MemberIds { get; private set; }
    }
}