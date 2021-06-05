using Mediator.Net.Contracts;
using SugarChat.Push.SignalR.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.Goup
{
    public class GroupCommand : ICommand
    {
        public GroupAction Action { get; set; }
        public string GroupName { get; set; }
        public string UserIdentifier { get; set; }
    }
}
