using SugarChat.Push.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Models
{
    public class SendCustomMessageModel
    {
        public SendWay SendWay { get; set; }

        public string Method { get; set; }

        public string[] Messages { get; set; }

        public string SendTo { get; set; }
    }
}
