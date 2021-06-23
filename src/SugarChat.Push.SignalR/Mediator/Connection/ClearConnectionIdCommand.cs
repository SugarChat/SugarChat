using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.Connection
{
    public class ClearConnectionIdCommand : ICommand
    {
        public string ConnectionId { get; set; }
    }
}
