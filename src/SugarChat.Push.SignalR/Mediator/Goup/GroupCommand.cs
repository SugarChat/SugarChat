using Mediator.Net.Contracts;
using SugarChat.Push.SignalR.Enums;
using System.Collections.Generic;

namespace SugarChat.Push.SignalR.Mediator.Goup
{
    public class GroupCommand : ICommand
    {
        public GroupAction Action { get; set; }
        public IReadOnlyList<string> GroupNames { get; set; }
        public string UserIdentifier { get; set; }
    }
}
