using Mediator.Net.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Mediator.SendMessage
{
    public class SendMessageCommand : ICommand
    {
        public SendWay SendWay { get; set; }

        public string Method { get; set; }

        public string[] Messages { get; set; }

        public bool IsMass { get; set; }

        public IReadOnlyList<string> SendTos { get; set; }

        public string SendTo { get; set; }
    }
}
