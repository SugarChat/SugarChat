using SugarChat.Push.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SugarChat.Push.SignalR.Models
{
    public class SendMessageModel
    {
        public SendWay SendWay { get; set; }

        public object[] Messages { get; set; }

        public string SendTo { get; set; }
    }
}
