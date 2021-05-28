using SugarChat.Push.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.SignalR.Server.Models
{
    public class SendCustomMessageModel
    {
        public SendWay SendWay { get; set; }

        public string Method { get; set; }

        public object[] Messages { get; set; }

        public string SendTo { get; set; }
    }
}
