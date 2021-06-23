using SugarChat.SignalR.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Server.Models
{
    public class GroupActionModel
    {
        public GroupAction Action { get; set; }
        public IReadOnlyList<string> GroupNames { get; set; }
        public string UserIdentifier { get; set; }
    }
}
