using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Core.Domain
{
    public class GroupCustomProperty : Entity
    {
        public string GroupId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
